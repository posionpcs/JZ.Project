using System;
using System.IO;
using System.Net;
using System.Text;

namespace FrameWork.SMS
{
    public class Sms : ISms
    {
        private static string PostRequest(string url, string[] paramName, string[] paramValue)
        {
            StringBuilder builder = new StringBuilder(paramName[0] + "=" + paramValue[0]);
            for (int i = 1; i < paramName.Length; i++)
            {
                builder.Append("&" + paramName[i] + "=" + paramValue[i]);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }

        public ReplyResult[] PullReply()
        {
            throw new NotImplementedException();
        }

        public bool Send(string mobile, string content)
        {
            //mobile.ThrowIfNull<string>("手机");
            //content.ThrowIfNull<string>("短信内容");
            //new ServiceResult("消息发送");
            //try
            //{
            //    Message message2 = new Message
            //    {
            //        SendType = "SMS",
            //        Content = content,
            //        Title = content,
            //        SystemId = "SmsSystemId".ValueOfAppSetting(),
            //        To = mobile
            //    };
            //    string[] paramName = new string[] { "msg", "token", "time" };
            //    string str = HttpUtility.UrlEncode(JsonUtil.ToJson(message2, false));
            //    string[] paramValue = new string[] { str, "tokenId".ValueOfAppSetting(), DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") };
            //    return JsonUtil.FromJson<ResultMessage>(PostRequest("MsgUrl".ValueOfAppSetting(), paramName, paramValue)).Result;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return false;
        }

        [Serializable]
        public class Message
        {
            public string Cc { get; set; }

            public string Content { get; set; }

            public string SendType { get; set; }

            public string SystemId { get; set; }

            public string Title { get; set; }

            public string To { get; set; }
        }

        public class ResultMessage
        {
            public string Msg { get; set; }

            public bool Result { get; set; }
        }
    }
}
