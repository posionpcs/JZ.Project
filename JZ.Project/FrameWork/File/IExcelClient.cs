using System.Collections.Generic;
using System.Web;

namespace FrameWork.File
{
    public interface IExcelClient
    {
        void FileExport<T>(IEnumerable<T> source, string fileName);
        IEnumerable<T> FileImport<T>(string fileName) where T: new();
        void HttpExport<T>(IEnumerable<T> source, string fileName = "");
        IEnumerable<T> HttpImport<T>(HttpPostedFileBase postedFile) where T: new();
    }
}

