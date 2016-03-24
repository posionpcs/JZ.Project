namespace FrameWork
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ExceptionExtensions
    {
        public static void ThrowIf<T>(this T argument, Func<T, bool> predicate, string msg)
        {
            if (predicate(argument))
            {
                throw new ArgumentException(msg);
            }
        }

        public static void ThrowIfNull<T>(this T argument, string paramName) where T: class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}

