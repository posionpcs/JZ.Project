namespace Framework.DAL.SqlServer
{
    using System;
    using System.Data;
    using System.Runtime.CompilerServices;

    public class UnitAction
    {
        public UnitAction(Func<IDbTransaction, int> action, IDbConnection conn)
        {
            this.Conn = conn;
            this.Action = action;
        }

        public Func<IDbTransaction, int> Action { get; set; }

        public IDbConnection Conn { get; set; }
    }
}

