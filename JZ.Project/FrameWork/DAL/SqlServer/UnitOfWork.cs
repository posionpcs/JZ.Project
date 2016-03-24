using System.Collections.Generic;
using System.Linq;
using FrameWork.DAL;

namespace Framework.DAL.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private List<UnitTransaction> transList = new List<UnitTransaction>();

        public IUnitTransaction BeginTransaction()
        {
            UnitTransaction item = new UnitTransaction(this, transList.Count);
            transList.Add(item);
            return item;
        }

        public void Clear()
        {
            transList.Clear();
        }

        public List<UnitAction> GetActionList()
        {
            return (from t in transList
                    where t.Commited
                    select t).SelectMany(t => t.ActionList).ToList();
        }
    }
}

