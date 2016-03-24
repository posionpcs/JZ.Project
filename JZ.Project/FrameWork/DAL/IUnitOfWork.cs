namespace FrameWork.DAL
{
    public interface IUnitOfWork
    {
        IUnitTransaction BeginTransaction();
    }
}

