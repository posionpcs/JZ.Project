using Dapper;

namespace Framework.DAL.SqlServer
{
    public class SqlCmd
    {
        public SqlCmd()
        {
            this.Parameters = new DynamicParameters();
        }
        public DynamicParameters Parameters { get; set; }
        public string Sql { get; set; }
    }
}

