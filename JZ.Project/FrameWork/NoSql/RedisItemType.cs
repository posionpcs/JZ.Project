namespace FrameWork.NoSql
{
    using System;

    public enum RedisItemType
    {
        None,
        String,
        List,
        Set,
        SortedSet,
        Hash,
        Unknown
    }
}

