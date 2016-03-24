namespace FrameWork.NoSql
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class MongoEntity
    {
        public MongoEntity()
        {
            this._id = Guid.NewGuid().ToString("N");
        }

        public string _id { get; set; }
    }
}

