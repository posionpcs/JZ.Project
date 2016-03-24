using FrameWork.NoSql;

namespace FrameWork.Log
{
    public interface ILog
    {
        void WriteToFile(string message, string dir = "");
        void WriteToMongo<T>(T message, string table) where T: MongoEntity;
        void WriteToMQ<T>(string queue, string message);
    }
}

