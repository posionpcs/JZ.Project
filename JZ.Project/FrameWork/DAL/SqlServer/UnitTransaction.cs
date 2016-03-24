using FrameWork;
using FrameWork.DAL;

namespace Framework.DAL.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
 
    using System.Transactions;

    public class UnitTransaction : IUnitTransaction, IDisposable
    {
        private List<UnitAction> actionList = new List<UnitAction>();
        private bool commited;
        private int depth;
        private UnitOfWork uow;

        public UnitTransaction(UnitOfWork uow, int depth)
        {
            this.uow = uow;
            this.depth = depth;
        }

        public int Commit(TransactionType tranType)
        {
            int num;
            if (this.commited)
            {
                throw new InvalidOperationException("IUnitTransaction 只能提交一次，请勿重复提交");
            }
            this.commited = true;
            if (this.depth > 0)
            {
                return 0;
            }
            List<UnitAction> actionList = this.uow.GetActionList();
            if ((actionList == null) || (actionList.Count == 0))
            {
                return 0;
            }
            try
            {
                num = this.Commit(actionList, tranType);
            }
            finally
            {
                this.uow.Clear();
            }
            return num;
        }

        private int Commit(List<UnitAction> actionList, TransactionType type)
        {
            if (type == TransactionType.None)
            {
                return this.CommitWithNoTran(actionList);
            }
            if (this.InSameConnection(actionList))
            {
                if (type != TransactionType.Local)
                {
                    throw new Exception("操作不在同一个连接对象，不能使用本地事务方式提交", null);
                }
                return this.CommitWithLocalTran(actionList);
            }
            if (type == TransactionType.Distribute)
            {
                return this.CommitWithDistributedTran(actionList);
            }
            if (type != TransactionType.Compensate)
            {
                throw new Exception("不同 Connection 事务不能以 {0} 方式提交".Fmt(new object[] { type }), null);
            }
            return this.CommitWithCompensatedTran(actionList);
        }

        private int CommitWithCompensatedTran(List<UnitAction> actionList)
        {
            throw new NotImplementedException();
        }

        private int CommitWithDistributedTran(List<UnitAction> actionList)
        {
            using (new TransactionScope())
            {
                foreach (UnitAction action in actionList)
                {
                    action.Action(null);
                }
            }
            return 0;
        }

        private int CommitWithLocalTran(List<UnitAction> actionList)
        {
            int num = 0;
            IDbConnection conn = actionList.First<UnitAction>(a => (a.Conn != null)).Conn;
            conn.Open();
            using (IDbTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    try
                    {
                        foreach (UnitAction action in actionList)
                        {
                            num += action.Action(transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        throw new Exception(exception.Message, exception.InnerException);
                    }
                    return num;
                }
                finally
                {
                    conn.Close();
                }
            }
            return num;
        }

        private int CommitWithNoTran(List<UnitAction> actionList)
        {
            int num = 0;
            foreach (UnitAction action in actionList)
            {
                num += action.Action(null);
            }
            return num;
        }

        public void Dispose()
        {
            if (this.depth == 0)
            {
                this.uow.Clear();
            }
        }

        private bool InSameConnection(List<UnitAction> actionList)
        {
            int hashCode = actionList.First<UnitAction>(a => (a.Conn != null)).Conn.GetHashCode();
            foreach (UnitAction action in actionList)
            {
                if ((action.Conn != null) && (action.Conn.GetHashCode() != hashCode))
                {
                    return false;
                }
            }
            return true;
        }

        public void Register(Action action)
        {
            this.actionList.Add(new UnitAction(delegate (IDbTransaction tran) {
                action();
                return 0;
            }, null));
        }

        public void Register(Func<IDbTransaction, int> action, IDbConnection conn)
        {
            this.actionList.Add(new UnitAction(action, conn));
        }

        public List<UnitAction> ActionList
        {
            get
            {
                return this.actionList;
            }
        }

        public bool Commited
        {
            get
            {
                return this.commited;
            }
        }

  
    }
}

