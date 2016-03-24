namespace FrameWork.NoSql
{
    using System;

    [Flags]
    public enum RedisExclude
    {
        None,
        Start,
        Stop,
        Both
    }
}

