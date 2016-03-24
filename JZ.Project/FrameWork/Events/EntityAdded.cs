namespace FrameWork.Events
{
    public class EntityAdded<T> where T: class
    {
        public EntityAdded(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}

