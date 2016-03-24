namespace FrameWork.Events
{
    using System;
    using System.Runtime.CompilerServices;

    public class EntityAdded<T> where T: class
    {
        public EntityAdded(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}

