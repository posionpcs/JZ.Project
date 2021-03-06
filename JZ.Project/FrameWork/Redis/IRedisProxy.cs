﻿namespace FrameWork.Redis
{
    using FrameWork.Caching;
    using FrameWork.NoSql;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public interface IRedisProxy : ICache
    {
        Task<bool> AddAsync(KeyValuePair<string, string>[] kvs);
        Task<bool> AddAsync(string key, string value, int seconds = 0);
        Task<bool> ContainsAsync(string key);
        long Decrement(string key, long value);
        Task<long> DecrementAsync(string key, long value);
        Task<string> GetAsync(string key);
        Task<string[]> GetAsync(string[] keys);
        Task<string> GetOrAddAsync(string key, Func<string> aquire, int seconds = 0);
        Task<string> GetOrSetAsync(string key, Func<string> aquire, int seconds = 0);
        string GetSet(string key, string value);
        Task<string> GetSetAsync(string key, string value);
        long HashDecrement(string key, string field, long value = 1L);
        Task<long> HashDecrementAsync(string key, string field, long value = 1L);
        bool HashDelete(string key, string field);
        long HashDelete(string key, string[] fields);
        Task<bool> HashDeleteAsync(string key, string field);
        Task<long> HashDeleteAsync(string key, string[] fields);
        bool HashExists(string key, string field);
        Task<bool> HashExistsAsync(string key, string field);
        string HashGet(string key, string field);
        KeyValuePair<string, string>[] HashGetAll(string key);
        Task<KeyValuePair<string, string>[]> HashGetAllAsync(string key);
        Task<string> HashGetAsync(string key, string field);
        long HashIncrement(string key, string field, long value = 1L);
        Task<long> HashIncrementAsync(string key, string field, long value = 1L);
        string[] HashKeys(string key);
        Task<string[]> HashKeysAsync(string key);
        long HashLength(string key);
        Task<long> HashLengthAsync(string key);
        KeyValuePair<string, string>[] HashScan(string key, string pattern = null, int pageSize = 10, long cursor = 0L, int pageOffset = 0);
        string[] HashValues(string key);
        Task<string[]> HashValuesAsync(string key);
        long Increment(string key, long value);
        Task<long> IncrementAsync(string key, long value);
        bool KeyExpire(string key, DateTime? expiry);
        Task<bool> KeyExpireAsync(string key, DateTime? expiry);
        TimeSpan? KeyTimeToLive(string key);
        Task<TimeSpan?> KeyTimeToLiveAsync(string key);
        RedisItemType KeyType(string key);
        Task<RedisItemType> KeyTypeAsync(string key);
        string ListGetByIndex(string key, long index);
        Task<string> ListGetByIndexAsync(string key, long index);
        long ListInsertAfter(string key, string pivot, string value);
        Task<long> ListInsertAfterAsync(string key, string pivot, string value);
        long ListInsertBefore(string key, string pivot, string value);
        Task<long> ListInsertBeforeAsync(string key, string pivot, string value);
        string ListLeftPop(string key);
        Task<string> ListLeftPopAsync(string key);
        long ListLeftPush(string key, string value);
        long ListLeftPush(string key, string[] values);
        Task<long> ListLeftPushAsync(string key, string value);
        Task<long> ListLeftPushAsync(string key, string[] values);
        long ListLength(string key);
        Task<long> ListLengthAsync(string key);
        string[] ListRange(string key, long start = 0L, long stop = -1L);
        Task<string[]> ListRangeAsync(string key, long start = 0L, long stop = -1L);
        long ListRemove(string key, string value, long count = 0L);
        Task<long> ListRemoveAsync(string key, string value, long count = 0L);
        string ListRightPop(string key);
        Task<string> ListRightPopAsync(string key);
        long ListRightPush(string key, string value);
        long ListRightPush(string key, string[] values);
        Task<long> ListRightPushAsync(string key, string value);
        Task<long> ListRightPushAsync(string key, string[] values);
        void ListSetByIndex(string key, long index, string value);
        Task ListSetByIndexAsync(string key, long index, string value);
        void ListTrim(string key, long start, long stop);
        Task ListTrimAsync(string key, long start, long stop);
        long Publish(string key, string message);
        Task<long> PublishAsync(string key, string message);
        Task<bool> RemoveAsync(string key);
        string ScriptEvaluate(string script, object args = null);
        bool SetAdd(string key, string value);
        long SetAdd(string key, string[] values);
        Task<bool> SetAddAsync(string key, string value);
        Task<long> SetAddAsync(string key, string[] values);
        Task<bool> SetAsync(KeyValuePair<string, string>[] kvs);
        Task<bool> SetAsync(string key, string value, int seconds = 0);
        string[] SetCombine(RedisSetOperation operation, string[] keys);
        string[] SetCombine(RedisSetOperation operation, string first, string second);
        long SetCombineAndStore(RedisSetOperation operation, string desctination, string[] keys);
        long SetCombineAndStore(RedisSetOperation operation, string desctination, string first, string second);
        Task<long> SetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string[] keys);
        Task<long> SetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string first, string second);
        Task<string[]> SetCombineAsync(RedisSetOperation operation, string[] keys);
        Task<string[]> SetCombineAsync(RedisSetOperation operation, string first, string second);
        bool SetContains(string key, string value);
        Task<bool> SetContainsAsync(string key, string value);
        long SetLength(string key);
        Task<long> SetLengthAsync(string key);
        string[] SetMembers(string key);
        Task<string[]> SetMembersAsync(string key);
        bool SetMove(string source, string desctination, string value);
        Task<bool> SetMoveAsync(string source, string desctination, string value);
        string SetPop(string key);
        Task<string> SetPopAsync(string key);
        string SetRandomMember(string key);
        Task<string> SetRandomMemberAsync(string key);
        string[] SetRandomMembers(string key, long count);
        Task<string[]> SetRandomMembersAsync(string key, long count);
        bool SetRemove(string key, string value);
        long SetRemove(string key, string[] values);
        Task<bool> SetRemoveAsync(string key, string value);
        Task<long> SetRemoveAsync(string key, string[] values);
        string[] SetScan(string key, string pattern = null, int pageSize = 0, long cursor = 0L, int pageOffset = 0);
        long SortedSetAdd(string key, KeyValuePair<string, double>[] members);
        bool SortedSetAdd(string key, string member, double score);
        Task<long> SortedSetAddAsync(string key, KeyValuePair<string, double>[] members);
        Task<bool> SortedSetAddAsync(string key, string member, double score);
        long SortedSetCombineAndStore(RedisSetOperation operation, string desctination, string first, string second, RedisAggregate aggregate = 0);
        long SortedSetCombineAndStore(RedisSetOperation operation, string desctination, string[] keys, double[] weights = null, RedisAggregate aggregate = 0);
        Task<long> SortedSetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string first, string second, RedisAggregate aggregate = 0);
        Task<long> SortedSetCombineAndStoreAsync(RedisSetOperation operation, string desctination, string[] keys, double[] weights = null, RedisAggregate aggregate = 0);
        double SortedSetDecrement(string key, string member, double value);
        Task<double> SortedSetDecrementAsync(string key, string member, double value);
        double SortedSetIncrement(string key, string member, double value);
        Task<double> SortedSetIncrementAsync(string key, string member, double value);
        long SortedSetLength(string key, double min = (double) -1.0 / (double) 0.0, double max = (double) 1.0 / (double) 0.0, RedisExclude exclude = 0);
        Task<long> SortedSetLengthAsync(string key, double min = (double) -1.0 / (double) 0.0, double max = (double) 1.0 / (double) 0.0, RedisExclude exclude = 0);
        long SortedSetLengthByValue(string key, string min, string max, RedisExclude exclude = 0);
        Task<long> SortedSetLengthByValueAsync(string key, string min, string max, RedisExclude exclude = 0);
        string[] SortedSetRangeByRank(string key, long start = 0L, long stop = -1L, RedisOrder order = 0);
        Task<string[]> SortedSetRangeByRankAsync(string key, long start = 0L, long stop = -1L, RedisOrder order = 0);
        KeyValuePair<string, double>[] SortedSetRangeByRankWithScores(string key, long start = 0L, long stop = -1L, RedisOrder order = 0);
        Task<KeyValuePair<string, double>[]> SortedSetRangeByRankWithScoresAsync(string key, long start = 0L, long stop = -1L, RedisOrder order = 0);
        string[] SortedSetRangeByScore(string key, double start = (double) -1.0 / (double) 0.0, double stop = (double) 1.0 / (double) 0.0, RedisExclude exclude = 0, RedisOrder order = 0, long skip = 0L, long take = -1L);
        Task<string[]> SortedSetRangeByScoreAsync(string key, double start = (double) -1.0 / (double) 0.0, double stop = (double) 1.0 / (double) 0.0, RedisExclude exclude = 0, RedisOrder order = 0, long skip = 0L, long take = -1L);
        KeyValuePair<string, double>[] SortedSetRangeByScoreWithScores(string key, double start = (double) -1.0 / (double) 0.0, double stop = (double) 1.0 / (double) 0.0, RedisExclude exclude = 0, RedisOrder order = 0, long skip = 0L, long take = -1L);
        Task<KeyValuePair<string, double>[]> SortedSetRangeByScoreWithScoresAsync(string key, double start = (double) -1.0 / (double) 0.0, double stop = (double) 1.0 / (double) 0.0, RedisExclude exclude = 0, RedisOrder order = 0, long skip = 0L, long take = -1L);
        string[] SortedSetRangeByValue(string key, string min = null, string max = null, RedisExclude exclude = 0, long skip = 0L, long take = -1L);
        Task<string[]> SortedSetRangeByValueAsync(string key, string min = null, string max = null, RedisExclude exclude = 0, long skip = 0L, long take = -1L);
        long? SortedSetRank(string key, string member, RedisOrder order = 0);
        Task<long?> SortedSetRankAsync(string key, string member, RedisOrder order = 0);
        bool SortedSetRemove(string key, string member);
        long SortedSetRemove(string key, string[] members);
        Task<bool> SortedSetRemoveAsync(string key, string member);
        Task<long> SortedSetRemoveAsync(string key, string[] members);
        long SortedSetRemoveRangeByRank(string key, long start, long stop);
        Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop);
        long SortedSetRemoveRangeByScore(string key, double start, double stop, RedisExclude exclude = 0);
        Task<long> SortedSetRemoveRangeByScoreAsync(string key, double start, double stop, RedisExclude exclude = 0);
        long SortedSetRemoveRangeByValue(string key, string min, string max, RedisExclude exclude = 0);
        Task<long> SortedSetRemoveRangeByValueAsync(string key, string min, string max, RedisExclude exclude = 0);
        KeyValuePair<string, double>[] SortedSetScan(string key, string pattern = null, int pageSize = 10, int cursor = 0, int pageOffset = 0);
        double? SortedSetScore(string key, string member);
        Task<double?> SortedSetScoreAsync(string key, string member);
        void Subscribe(string key, Action<string, string> process);
        T Wait<T>(Task<T> task);
    }
}

