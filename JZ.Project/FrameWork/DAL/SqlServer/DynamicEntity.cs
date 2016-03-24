using System.Reflection;
using System.Text;

namespace Framework.DAL.SqlServer
{
    public static class DynamicEntity<T>
    {
        static DynamicEntity()
        {
            Fields = typeof(T).GetProperties();
            SetSqlFields();
            SetInsertedFields();
        }
        public static PropertyInfo[] Fields { get; set; }
        public static string InsertedSqlFields { get; set; }
        public static string SqlFields { get; set; }

        /// <summary>
        /// 设置新增的字段
        /// </summary>
        private static void SetInsertedFields()
        {
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            foreach (PropertyInfo info in Fields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append("INSERTED.");
                builder.Append(info.Name);
                flag = true;
            }
            InsertedSqlFields = builder.ToString();
        }


        /// <summary>
        /// 设置sql的字段
        /// </summary>
        private static void SetSqlFields()
        {
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            foreach (PropertyInfo info in Fields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append(info.Name);
                flag = true;
            }
            SqlFields = builder.ToString();
        }
    }
}

