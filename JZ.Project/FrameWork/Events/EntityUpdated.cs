﻿namespace FrameWork.Events
{
    using System;
    using System.Runtime.CompilerServices;

    public class EntityUpdated<T> where T: class
    {
        public EntityUpdated(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}

