namespace FrameWork.Events
{
    using System;
    using System.Runtime.CompilerServices;

    public class EntityDeleted<T> where T: class
    {
        public EntityDeleted(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}

