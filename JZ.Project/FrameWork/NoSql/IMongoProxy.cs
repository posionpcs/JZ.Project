namespace FrameWork.NoSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public interface IMongoProxy
    {
        long Count<T>(string database, string collection, Expression<Func<T, bool>> predicate) where T: MongoEntity;
        Task<long> CountAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate) where T: MongoEntity;
        long DeleteMany<T>(string database, string collection, Expression<Func<T, bool>> predicate) where T: MongoEntity;
        Task<long> DeleteManyAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate) where T: MongoEntity;
        long DeleteOne<T>(string database, string collection, Expression<Func<T, bool>> predicate) where T: MongoEntity;
        long DeleteOne<T>(string database, string collection, T entity) where T: MongoEntity;
        Task<long> DeleteOneAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate) where T: MongoEntity;
        Task<long> DeleteOneAsync<T>(string database, string collection, T entity) where T: MongoEntity;
        void DropCollection(string database, string collection);
        Task DropCollectionAsync(string database, string collection);
        void DropDatabase(string database);
        Task DropDatabaseAsync(string database);
        T Get<T>(string database, string collection, Expression<Func<T, bool>> predicate);
        TResult Get<T, TResult>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projector);
        Task<T> GetAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate);
        Task<TResult> GetAsync<T, TResult>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projector);
        void InsertMany<T>(string database, string collection, IEnumerable<T> entities) where T: MongoEntity;
        Task InsertManyAsync<T>(string database, string collection, IEnumerable<T> entities) where T: MongoEntity;
        void InsertOne<T>(string database, string collection, T entity) where T: MongoEntity;
        Task InsertOneAsync<T>(string database, string collection, T entity) where T: MongoEntity;
        List<string> ListConnections(string database);
        Task<List<string>> ListConnectionsAsync(string database);
        List<string> ListDatabase();
        Task<List<string>> ListDatabasesAsync();
        Task<PageList<object>> PageListAsync(string database, string collection, string json, string order, int pageIndex, int pageSize);
        Task<PageList<T>> PageListAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate, int pageIndex = 1, int pageSize = 20, Expression<Func<T, object>> orderby = null, bool desc = false) where T: MongoEntity;
        List<T> ToList<T>(string database, string collection, Expression<Func<T, bool>> predicate);
        List<TResult> ToList<T, TResult>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projector);
        Task<List<T>> ToListAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate);
        Task<List<TResult>> ToListAsync<T, TResult>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projector);
        int UpdateOne<T>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression);
        Task<int> UpdateOneAsync<T>(string database, string collection, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression);
    }
}

