namespace FrameWork.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface ICache
    {
        bool Add(string key, string value, int seconds = 0);
        bool Contains(string key);
        string Get(string key);
        string[] Get(string[] keys);
        string GetOrAdd(string key, Func<string> aquire, int seconds = 0);
        string GetOrSet(string key, Func<string> aquire, int seconds = 0);
        bool Remove(string key);
        long Remove(string[] keys);
        bool Set(KeyValuePair<string, string>[] kvs);
        bool Set(string key, string value, int seconds = 0);
    }
}

