namespace FrameWork.DAL
{
    using System;
    public interface IUnitTransaction : IDisposable
    {
        int Commit(TransactionType tranType);
        void Register(Action action);
    }
}

