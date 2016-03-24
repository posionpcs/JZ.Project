namespace QD.Web.AppApi.Common
{
    /// <summary>
    /// 文件上传接口返回类型
    /// </summary>
    public class FileResultResponse
    {
        /// <summary>
        /// 标识码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 具体结果
        /// </summary>
        public object Result { get; set; }
    }

    /// <summary>
    /// 上传文件信息类
    /// </summary>
    public class UploadFileInfo
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 文件原名称
        /// </summary>
        public string originalName { get; set; }
        /// <summary>
        /// 文件url
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 文件大小 单位byte
        /// </summary>
        public int size { get; set; }
    }
}