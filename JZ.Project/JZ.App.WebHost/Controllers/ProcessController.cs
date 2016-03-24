using QD.Web.AppApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QD.Framework;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Web.Routing;
using System.Web;
using QD.Web.AppApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using log4net;
using QD.Framework.DAL;
using QD.Models.Project;
using QD.Framework.ESB;
using QD.Web.AppApi.App_Start;
using QD.BUS.Message.Log;
using QD.Models.QianTu;
using QD.Service.QianTu;
using System.Diagnostics;

namespace QD.Web.AppApi.Controllers
{
    public class ProcessController : ApiController
    {
        private static ILog log = LogManager.GetLogger(typeof(ProcessController));
        private IServiceBus bus;
        private IProcessorFactory factory;

        public ProcessController(
            IProcessorFactory factory,
            IServiceBus bus)
        {
            this.factory = factory;
            this.bus = bus;
        }

        [HttpPost]
        [HttpGet]
        public BaseResponse Index()
        {
            var watcher = new Stopwatch();
            watcher.Start();

            var response = new BaseResponse();
            BaseResponse exResponse = null;

            var requestId = string.Empty;
            var requestDataJson = string.Empty;
            var userDataJson = string.Empty;
            var userData = new UserData();
            var requestData = new RequestData();
            var logMsg = new ApiLogMessage();
            var bizCode = string.Empty;

            try
            {

                requestId = HttpContext.Current.Request.Params["RequestId"];
                var parmUserData = HttpContext.Current.Request.Params["UserData"];
                var parmRequestData = HttpContext.Current.Request.Params["RequestData"];

                logMsg.UserDataStr = parmUserData;
                logMsg.RequestDataStr = parmRequestData;

                userDataJson = Base64Utils.DecodeBase64String(parmUserData);
                userData = GetObject<UserData>(userDataJson);
                requestDataJson = Base64Utils.DecodeBase64String(parmRequestData);
                requestData = GetObject<RequestData>(requestDataJson);

                logMsg.RequestId = requestId;
                logMsg.LogTime = DateTime.Now;

                if (!requestData.IsNull())
                {
                    logMsg.Cmd = requestData.cmd;
                }

                if (!userData.IsNull())
                {
                    logMsg.UserData = new ApiLogUserData
                    {
                        UserId = userData.UserId,
                        Mobile = userData.Mobile,
                        RealName = userData.RealName,
                        NickName = userData.NickName,
                        IDCard = userData.IDCard,
                        EPlusAccountId = userData.EPlusAccountId,
                        InstitutionNo = userData.InstitutionNo,
                        Usrnbr = userData.Usrnbr
                    };
                }

                logMsg.RequestJson = requestDataJson;

                if (requestData == null)
                {
                    response = BaseResponse.Create(ApiEnum.ResponseCode.解析报文错误, null, 0);
                }
                else
                {
                    var cmd = requestData.cmd;
                    bizCode = ProcessorUtil.GetBizCode(cmd);

                    var regex = new Regex(@"\S+.\S+", RegexOptions.Compiled);
                    if (!regex.IsMatch(cmd) || string.IsNullOrEmpty(bizCode))
                        response = BaseResponse.Create(ApiEnum.ResponseCode.无效交易类型, string.Format("未定义接口{0}的请求类", bizCode), null, 0);
                    else
                    {
                        var baseRequest = ProcessorUtil.GetRequest(bizCode, Convert.ToString(requestData.body));
                        if (baseRequest == null)
                            response = BaseResponse.Create(ApiEnum.ResponseCode.参数不正确, 0);
                        else
                        {
                            //baseRequest.RequestData = requestData;
                            baseRequest.UserData = userData;
                            baseRequest.Head = requestData.head;
                            baseRequest.RequestId = requestId;
                            baseRequest.Cmd = cmd;

                            var errMsg = "";
                            if (!ModelVerify(baseRequest, out errMsg))
                                return BaseResponse.Create(ApiEnum.ResponseCode.参数不正确, errMsg, null, 0);

                            var processor = this.factory.Create(bizCode);
                            response = processor.Process(baseRequest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                response = BaseResponse.Create(ApiEnum.ResponseCode.系统内部错误, "服务器开小差，请稍后重试", 0);
                exResponse = BaseResponse.Create(ApiEnum.ResponseCode.系统内部错误, ex.ToString(), 0);
                logMsg.IsError = true;
            }
            finally
            {
                WriteRequestInfo(userData, requestData, requestId, bizCode);

                watcher.Stop();
                var duration = watcher.Elapsed.TotalMilliseconds;

                var logStr = string.Empty;
                logStr += string.Format("【请求报文】RequestId：{0}", requestId) + Environment.NewLine;
                logStr += string.Format("UserData：{0}", userDataJson) + Environment.NewLine;
                logStr += string.Format("RequestData：{0}", requestDataJson) + Environment.NewLine;
                logStr += string.Format("【响应报文】{0}", response.ToJson());
                logStr += string.Format("【耗时】{0}毫秒", duration);
                log.Info(logStr.ToString());


                logMsg.Response = exResponse.IsNull() ? response.ToJson() : exResponse.ToJson();
                logMsg.Duration = duration;
                if (AppConfig.LogType == LogType.MQ)
                {
                    try
                    {
                        this.bus.Publish(logMsg);
                    }
                    catch (Exception ex)
                    {
                        log.Error("写入MQ失败，RequestId：{0}\r\n{1}".Fmt(requestData, ex.ToString()));
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// 添加设备信息记录
        /// </summary>
        /// <param name="curUser">当前用户对象</param>
        /// <param name="reqData">当前请求数据对象</param>
        /// <param name="requestId">requestId</param>
        /// <param name="apiCode">api业务代码</param>
        private void WriteRequestInfo(UserData curUser, RequestData reqData, string requestId, string apiCode)
        {
            try
            {
                if (curUser == null) curUser = new UserData();
                if (reqData == null) reqData = new RequestData();
                if (reqData.head == null) reqData.head = new Head();

                var model = new EquipmentInfoMessage()
                {
                    Id = Guid.NewGuid().ToString(),
                    EName = reqData.head.clientModel,
                    IMEI = reqData.head.iMEI,
                    RequestId = requestId,
                    UId = curUser.UserId,
                    UName = curUser.RealName,
                    Mobile = curUser.Mobile,
                    CreateTime = DateTime.Now,
                    Operation = ApiEnumsHelp.OperationType.Get(apiCode),
                    ApiCode = apiCode,
                    Longitude = reqData.head.longitude,
                    Latitude = reqData.head.latitude,
                    IP = reqData.head.ip,
                    Address = reqData.head.location,
                    IsHighAnonyProxy = false
                };
                bus.Publish(model);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("用户请求信息记录失败！Error：{0}。", ex.Message));
            }
        }

        #region
        private T GetObject<T>(string jsonStr)
        {
            var res = default(T);
            try
            {
                if (!string.IsNullOrWhiteSpace(jsonStr))
                {
                    res = jsonStr.FromJson<T>();
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }

        private bool ModelVerify(object model, out string msg)
        {
            msg = "";
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, context, results, true);
            var errMsg = new StringBuilder();
            if (!isValid)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    ValidationResult r = results[i];
                    errMsg.Append(r.ErrorMessage + ";");
                }
                msg = errMsg.ToString();
                return false;
            }
            return true;
        }
        #endregion
    }
}
