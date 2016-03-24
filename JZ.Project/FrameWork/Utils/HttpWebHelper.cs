using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FrameWork.Utils
{
    public class HttpWebHelper
    {
        private CookieCollection _cookies = new CookieCollection();
        private static HttpWebHelper _Helper;
        private static readonly string DefaultUserAgent =
            "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// 清除Cookie
        /// </summary>
        public void ClearCookies()
        {
            this._cookies = new CookieCollection();
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string CraeteParameter(IDictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in parameters.Keys)
            {
                builder.AppendFormat("&{0}={1}", str, parameters[str]);
            }
            return builder.ToString().TrimStart(new char[] {'&'});
        }

       
        public HttpWebResponse CreateGetHttpResponse(string url, int? timeout = 300, string userAgent = "",
            CookieCollection cookies = null, string Referer = "", Dictionary<string, string> headers = null,
            string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string Accept = "*/*")
        {
            HttpWebRequest request;
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(HttpWebHelper.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (this.Proxy != null)
            {
                request.Proxy = this.Proxy;
            }
            request.Method = "GET";
            request.Headers["Pragma"] = "no-cache";
            request.Accept = Accept;
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.ContentType = contentType;
            request.UserAgent = DefaultUserAgent;
            request.Referer = Referer;
            if (keepAlive.HasValue)
            {
                request.KeepAlive = keepAlive.Value;
            }
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> pair in headers)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value*0x3e8;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            else
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(this.Cookies);
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("http://" + new Uri(url).Host)));
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("https://" + new Uri(url).Host)));
            this.Cookies.Add(response.Cookies);
            return response;
        }

        public HttpWebResponse CreatePostFileHttpResponse(string url, string filePath, int? timeout = 300,
            string userAgent = "", CookieCollection cookies = null, string Referer = "",
            Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded",
            bool? keepAlive = true, string Accept = "*/*")
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(HttpWebHelper.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (this.Proxy != null)
            {
                request.Proxy = this.Proxy;
            }
            request.Method = "POST";
            request.Accept = Accept;
            request.Referer = Referer;
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.UserAgent = DefaultUserAgent;
            request.ContentType = contentType;
            request.Headers["Pragma"] = "no-cache";
            if (keepAlive.HasValue)
            {
                request.KeepAlive = keepAlive.Value;
            }
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> pair in headers)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            else
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(this.Cookies);
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value*0x3e8;
            }
            request.Expect = string.Empty;
            if (!string.IsNullOrEmpty(filePath))
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    new BinaryReader(stream);
                    string str = "----------" + DateTime.Now.Ticks.ToString("x");
                    byte[] bytes = Encoding.ASCII.GetBytes("\r\n--" + str + "\r\n");
                    StringBuilder builder = new StringBuilder();
                    builder.Append("--");
                    builder.Append(str);
                    builder.Append("\r\n");
                    builder.Append("Content-Disposition: form-data; name=\"");
                    builder.Append("file");
                    builder.Append("\"; filename=\"");
                    builder.Append(stream.Name);
                    builder.Append("\"");
                    builder.Append("\r\n");
                    builder.Append("Content-Type: ");
                    builder.Append("application/octet-stream");
                    builder.Append("\r\n");
                    builder.Append("\r\n");
                    string s = builder.ToString();
                    byte[] buffer = Encoding.UTF8.GetBytes(s);
                    request.ContentType = "multipart/form-data; boundary=" + str;
                    long num = (stream.Length + buffer.Length) + bytes.Length;
                    request.ContentLength = num;
                    DateTime now = DateTime.Now;
                    byte[] buffer3 = new byte[stream.Length];
                    stream.Read(buffer3, 0, buffer3.Length);
                    using (Stream stream2 = request.GetRequestStream())
                    {
                        stream2.Write(buffer, 0, buffer.Length);
                        stream2.Write(buffer3, 0, buffer3.Length);
                        stream2.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("http://" + new Uri(url).Host)));
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("https://" + new Uri(url).Host)));
            this.Cookies.Add(response.Cookies);
            return response;
        }

        public HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters,
            Encoding requestEncoding, int? timeout = 300, string userAgent = "", CookieCollection cookies = null,
            string Referer = "", Dictionary<string, string> headers = null,
            string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string Accept = "*/*")
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(HttpWebHelper.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (this.Proxy != null)
            {
                request.Proxy = this.Proxy;
            }
            request.Method = "POST";
            request.Accept = Accept;
            request.Referer = Referer;
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.UserAgent = DefaultUserAgent;
            request.ContentType = contentType;
            request.Headers["Pragma"] = "no-cache";
            if (keepAlive.HasValue)
            {
                request.KeepAlive = keepAlive.Value;
            }
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> pair in headers)
                {
                    request.Headers[pair.Key] = pair.Value;
                }
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            else
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(this.Cookies);
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value*0x3e8;
            }
            request.Expect = string.Empty;
            if ((parameters != null) && (parameters.Count != 0))
            {
                byte[] bytes = requestEncoding.GetBytes(CraeteParameter(parameters).ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("http://" + new Uri(url).Host)));
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("https://" + new Uri(url).Host)));
            this.Cookies.Add(response.Cookies);
            return response;
        }

        public HttpWebResponse CreatePostHttpResponse(string url, string parameters, Encoding requestEncoding,
            int? timeout = 300, string userAgent = "", CookieCollection cookies = null, string Referer = "",
            Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded",
            bool? keepAlive = true, string Accept = "*/*")
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(HttpWebHelper.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            if (this.Proxy != null)
            {
                request.Proxy = this.Proxy;
            }
            request.Method = "POST";
            request.Headers.Add("Accept-Language", "zh-CN,en-GB;q=0.5");
            request.Method = "POST";
            request.Accept = Accept;
            request.Referer = Referer;
            request.Headers["Accept-Language"] = "en-US,en;q=0.5";
            request.UserAgent = DefaultUserAgent;
            request.ContentType = contentType;
            request.Headers["Pragma"] = "no-cache";
            if (keepAlive.HasValue)
            {
                request.KeepAlive = keepAlive.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            else
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(this.Cookies);
            }
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> pair in headers)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value*0x3e8;
            }
            request.Expect = string.Empty;
            if (!string.IsNullOrEmpty(parameters))
            {
                byte[] bytes = requestEncoding.GetBytes(parameters);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("http://" + new Uri(url).Host)));
            this.Cookies.Add(request.CookieContainer.GetCookies(new Uri("https://" + new Uri(url).Host)));
            this.Cookies.Add(response.Cookies);
            return response;
        }

        public string Get(string url, Encoding responseEncoding, int? timeout = 300, string userAgent = "",
            CookieCollection cookies = null, string Referer = "", Dictionary<string, string> headers = null,
            string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string Accept = "*/*")
        {
            string str;
            HttpWebResponse response = this.CreateGetHttpResponse(url, timeout, userAgent, cookies, Referer, headers,
                contentType, keepAlive, "*/*");
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                str = null;
            }
            return str;
        }

        public T Get<T>(string url, Encoding responseEncoding, int? timeout = 300, string userAgent = "",
            CookieCollection cookies = null, string Referer = "", Dictionary<string, string> headers = null,
            string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string Accept = "*/*")
        {
            return
                JsonConvert.DeserializeObject<T>(this.Get(url, responseEncoding, timeout, userAgent, cookies, Referer,
                    headers, contentType, keepAlive, "*/*"));
        }

        public byte[] GetFile(string url, out Dictionary<string, string> header, int? timeout = 300,
            string userAgent = "", CookieCollection cookies = null, string Referer = "",
            Dictionary<string, string> headers = null, bool? keepAlive = true, string Accept = "*/*")
        {
            HttpWebResponse response = this.CreateGetHttpResponse(url, timeout, userAgent, cookies, Referer, headers,
                "application/x-www-form-urlencoded", keepAlive, "*/*");
            header = new Dictionary<string, string>();
            foreach (string str in response.Headers.AllKeys)
            {
                header.Add(str, response.Headers[str]);
            }
            try
            {
                Stream responseStream = response.GetResponseStream();
                byte[] buffer = new byte[response.ContentLength];
                responseStream.Read(buffer, 0, buffer.Length);
                responseStream.Close();
                return buffer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Stream GetStream(string url, int? timeout = 300, string userAgent = "", CookieCollection cookies = null,
            string Referer = "", Dictionary<string, string> headers = null, bool? keepAlive = true,
            string Accept = "*/*")
        {
            return
                this.CreateGetHttpResponse(url, timeout, userAgent, cookies, Referer, headers,
                    "application/x-www-form-urlencoded", keepAlive, "*/*").GetResponseStream();
        }

        public string Post(string url, IDictionary<string, string> parameters, Encoding requestEncoding,
            Encoding responseEncoding, int? timeout = 300, string userAgent = "", CookieCollection cookies = null,
            string Referer = "", Dictionary<string, string> headers = null,
            string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string Accept = "*/*")
        {
            string str;
            HttpWebResponse response = this.CreatePostHttpResponse(url, parameters, requestEncoding, timeout, userAgent,
                cookies, Referer, headers, contentType, keepAlive, "*/*");
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                str = null;
            }
            return str;
        }

        public T Post<T>(string url, IDictionary<string, string> parameters, Encoding requestEncoding,
            Encoding responseEncoding, int? timeout = 300, string userAgent = "", CookieCollection cookies = null,
            string Referer = "", Dictionary<string, string> headers = null,
            string contentType = "application/x-www-form-urlencoded", bool? keepAlive = true, string Accept = "*/*")
        {
            return
                JsonConvert.DeserializeObject<T>(this.Post(url, parameters, requestEncoding, responseEncoding, timeout,
                    userAgent, cookies, Referer, headers, contentType, keepAlive, "*/*"));
        }

        public string Post(string url, string parameters, Encoding requestEncoding, Encoding responseEncoding,
            int? timeout = 300, string userAgent = "", CookieCollection cookies = null, string Referer = "",
            Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded",
            bool? keepAlive = true, string Accept = "*/*")
        {
            string str;
            HttpWebResponse response = this.CreatePostHttpResponse(url, parameters, requestEncoding, timeout, userAgent,
                cookies, Referer, headers, contentType, keepAlive, "*/*");
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                str = null;
            }
            return str;
        }

        public T Post<T>(string url, string parameters, Encoding requestEncoding, Encoding responseEncoding,
            int? timeout = 300, string userAgent = "", CookieCollection cookies = null, string Referer = "",
            Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded",
            bool? keepAlive = true, string Accept = "*/*")
        {
            return
                JsonConvert.DeserializeObject<T>(this.Post(url, parameters, requestEncoding, responseEncoding, timeout,
                    userAgent, cookies, Referer, headers, contentType, keepAlive, "*/*"));
        }

        public string PostFile(string url, string filePath, Encoding responseEncoding, int? timeout = 300,
            string userAgent = "", CookieCollection cookies = null, string Referer = "",
            Dictionary<string, string> headers = null, string contentType = "application/x-www-form-urlencoded",
            bool? keepAlive = true, string Accept = "*/*")
        {
            string str;
            HttpWebResponse response = this.CreatePostFileHttpResponse(url, filePath, timeout, userAgent, cookies,
                Referer, headers, contentType, keepAlive, "*/*");
            try
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncoding))
                {
                    str = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                str = null;
            }
            return str;
        }

        public CookieCollection Cookies
        {
            get { return this._cookies; }
        }

        public static HttpWebHelper Helper
        {
            get
            {
                if (_Helper == null)
                {
                    _Helper = new HttpWebHelper();
                }
                return _Helper;
            }
        }

        public WebProxy Proxy { get; set; }

    }

}