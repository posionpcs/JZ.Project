namespace FrameWork.File
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Web;

    public interface IExcelClient
    {
        void FileExport<T>(IEnumerable<T> source, string fileName);
        IEnumerable<T> FileImport<T>(string fileName) where T: new();
        void HttpExport<T>(IEnumerable<T> source, string fileName = "");
        IEnumerable<T> HttpImport<T>(HttpPostedFileBase postedFile) where T: new();
    }
}

