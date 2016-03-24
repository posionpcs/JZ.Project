namespace Framework.DAL.SqlServer
{
    using Dapper;
    using System;
    using System.Runtime.CompilerServices;

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

