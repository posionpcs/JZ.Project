namespace FrameWork.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Caching;
    using System.Runtime.InteropServices;

    public class LocalCache : ICache
    {
        public bool Add(string key, string value, int seconds = 0)
        {
            if (value == null)
            {
                return false;
            }
            return cache.Add(key, value, this.GetTimeOffset(seconds), null);
        }

        public bool Contains(string key)
        {
            return cache.Contains(key, null);
        }

        public string Get(string key)
        {
            return (cache.Get(key, null) as string);
        }

        public string[] Get(string[] keys)
        {
            if (keys == null)
            {
                return null;
            }
            string[] strArray = new string[keys.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = cache.Get(keys[i], null) as string;
            }
            return strArray;
        }

        public string GetOrAdd(string key, Func<string> aquire, int seconds = 0)
        {
            string str = cache.Get(key, null) as string;
            if (str == null)
            {
                str = aquire();
                if (str == null)
                {
                    return null;
                }
                cache.Add(key, str, this.GetTimeOffset(seconds), null);
            }
            return str;
        }

        public string GetOrSet(string key, Func<string> aquire, int seconds = 0)
        {
            string str = cache.Get(key, null) as string;
            if (str == null)
            {
                str = aquire();
                if (str == null)
                {
                    return null;
                }
                cache.Set(key, str, this.GetTimeOffset(seconds), null);
            }
            return str;
        }

        private DateTimeOffset GetTimeOffset(int seconds)
        {
            if (seconds <= 0)
            {
                return DateTimeOffset.MaxValue;
            }
            return DateTimeOffset.Now.AddSeconds((double)seconds);
        }

        public bool Remove(string key)
        {
            return (cache.Remove(key, null) != null);
        }

        public long Remove(string[] keys)
        {
            if (keys == null)
            {
                return 0L;
            }
            long num = 0L;
            foreach (string str in keys)
            {
                if (cache.Remove(str, null) != null)
                {
                    num += 1L;
                }
            }
            return num;
        }

        public bool Set(KeyValuePair<string, string>[] kvs)
        {
            if (kvs == null)
            {
                return false;
            }
            foreach (KeyValuePair<string, string> pair in kvs)
            {
                cache.Set(pair.Key, pair.Value, DateTimeOffset.MaxValue, null);
            }
            return true;
        }

        public bool Set(string key, string value, int seconds = 0)
        {
            if (value == null)
            {
                return false;
            }
            cache.Set(key, value, this.GetTimeOffset(seconds), null);
            return true;
        }

        private static ObjectCache cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }
    }
}

