using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net;

namespace QD.Web.AppApi.Common
{
    /// <summary>
    /// 提供常用的Web请求方法
    /// <Author>周维</Author>
    /// </summary>
    public static class RequestHelp
    {
        /// <summary>
        /// 构建多部分POST数据
        /// <Author>周维</Author>
        /// </summary>
        /// <param name="boundary">自定义边界</param>
        /// <param name="httpPostData">form方式提交的键值对</param>
        /// <param name="encoding">写入流使用的编码</param>
        /// <param name="uploadFiles">上载文件对象数组</param>
        /// <returns>返回请求流所需的byte数组</returns>
        private static byte[] BuildMultipartPostData(string boundary, Dictionary<string, string> httpPostData, Encoding encoding, params UploadFile[] uploadFiles)
        {
            StringBuilder requestInfo = new StringBuilder(500);
            if (httpPostData == null && uploadFiles.Length == 0)
            {
                requestInfo.AppendLine("--" + boundary);
                requestInfo.Append(Environment.NewLine);
            }
            if (httpPostData != null)
            {
                foreach (var item in httpPostData)
                {
                    requestInfo.AppendLine("--" + boundary);
                    requestInfo.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", item.Key));
                    requestInfo.Append(Environment.NewLine);
                    requestInfo.AppendLine(item.Value);
                }
            }
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            if (requestInfo.Length != 0)
                bw.Write(encoding.GetBytes(requestInfo.ToString()));
            foreach (var item in uploadFiles)
            {
                requestInfo.Clear();
                requestInfo.AppendLine("--" + boundary);
                requestInfo.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", item.Name, item.FileName));
                requestInfo.AppendLine(string.Format("Content-Type: {0}", item.ContentType));
                requestInfo.Append(Environment.NewLine);
                bw.Write(encoding.GetBytes(requestInfo.ToString()));
                bw.Write(item.FileBinary);
            }
            bw.Write(encoding.GetBytes(Environment.NewLine));
            bw.Write(encoding.GetBytes("--" + boundary + "--"));
            ms.Flush();
            ms.Position = 0;
            byte[] result = ms.ToArray();
            bw.Close();
            ms.Dispose();
            return result;
        }

        /// <summary>
        /// Form方式请求
        /// <Author>周维</Author>
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">请求方式</param>
        /// <param name="encoding">写入流使用的编码</param>
        /// <param name="httpPostData">方式提交的键值对</param>
        /// <param name="uploadFiles">上载文件对象数组</param>
        /// <returns>响应数据字符串</returns>
        public static string FormRequest(string url, string method, Encoding encoding, Dictionary<string, string> httpPostData, params UploadFile[] uploadFiles)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = null;
            string boundary = Convert.ToBase64String(encoding.GetBytes(Guid.NewGuid().ToString())) + DateTime.Now.Ticks.ToString();
            StreamReader sr = null;
            try
            {
                request.Method = method;
                request.Timeout = 150000;
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                byte[] multipartPostData = BuildMultipartPostData(boundary, httpPostData, encoding, uploadFiles);
                BinaryWriter bw = new BinaryWriter(request.GetRequestStream());
                bw.Write(multipartPostData);
                bw.Close();
                response = (HttpWebResponse)request.GetResponse();
                sr = new StreamReader(response.GetResponseStream());
                var responseData = sr.ReadToEnd();
                sr.Close();
                response.Close();
                return responseData;
            }
            catch (WebException eEx)
            {
                var resp = eEx.Response as HttpWebResponse;
                var respStr = "";
                if (resp != null)
                    respStr = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                string innerError = eEx.InnerException != null ? eEx.InnerException.Message : "";
                throw new Exception(string.Format("ErrorMessage:{0} \r\n InnerException:{1} \r\n ResponseBody:{2}", eEx.Message, innerError, respStr));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}