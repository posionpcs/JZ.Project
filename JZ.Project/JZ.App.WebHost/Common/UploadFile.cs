namespace QD.Web.AppApi.Common
{
    /// <summary>
    /// 上载文件数据实体模型
    /// <Author>周维</Author>
    /// </summary>
    public class UploadFile
    {
        /// <summary>
        /// 表单属性name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上载文件名称（文件名+扩展名）
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 上载文件的MIME类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文件的二进制
        /// </summary>
        public byte[] FileBinary { get; set; }

    }
}