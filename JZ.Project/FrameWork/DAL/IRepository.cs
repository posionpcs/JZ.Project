using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace FrameWork.DAL
{
    public interface IRepository<T> : IDisposable
    {
        int Add(T entity);
        int Add(IEnumerable<T> entities);
        void Add(T entity, IUnitTransaction tran);
        void Add(IEnumerable<T> entities, IUnitTransaction tran);
        int AddIfNotExists(T entity, Expression<Func<T, bool>> predicate);
        void AddIfNotExists(T entity, Expression<Func<T, bool>> predicate, IUnitTransaction tran);
        void BatchInsert(IEnumerable<T> entities, int timeout = 60);
        int Count(Expression<Func<T, bool>> predicate, string dbLock = "");
        int Delete(T entity);
        int Delete(Expression<Func<T, bool>> predicate);
        void Delete(T entity, IUnitTransaction tran);
        void Delete(Expression<Func<T, bool>> predicate, IUnitTransaction tran);
        int Execute(string sql, object parms);
        void Execute(string sql, object parms, IUnitTransaction tran);
        TResult ExecuteScalar<TResult>(string sql, object parms);
        bool Exists(Expression<Func<T, bool>> predicate, string dbLock = "");
        T Get(Expression<Func<T, bool>> predicate, string dbLock = "");
        T Get(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "");
        TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby);
        TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string dbLock);
        TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, string dbLock);
        PageList<T> PageList(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize);
        PageList<T> PageList(Expression<Func<T, bool>> predicate, string orderby, int pageIndex, int pageSize);
        PageList<T> PageList(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize, string dbLock);
        PageList<T> PageList(Expression<Func<T, bool>> predicate, string orderby, int pageIndex, int pageSize, string dbLock);
        PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize);
        PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, int pageIndex, int pageSize);
        PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize, string dbLock);
        PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, int pageIndex, int pageSize, string dbLock);
        IEnumerable<TResult> Query<TResult>(string sql, object parms);
        List<IEnumerable> Query<T1, T2>(string sql, object parms);
        List<IEnumerable> Query<T1, T2, T3>(string sql, object parms);
        List<IEnumerable> Query<T1, T2, T3, T4>(string sql, object parms);
        List<IEnumerable> Query<T1, T2, T3, T4, T5>(string sql, object parms);
        List<T> ToList(Expression<Func<T, bool>> predicate, string dbLock = "");
        List<T> ToList(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "");
        List<T> ToList(Expression<Func<T, bool>> predicate, int top, string dbLock = "");
        List<T> ToList(Expression<Func<T, bool>> predicate, int top, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "");
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby);
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top);
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string dbLock);
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, string dbLock);
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top, Func<DbSort<T>, DbSort<T>> orderby);
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top, string dbLock);
        List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top, Func<DbSort<T>, DbSort<T>> orderby, string dbLock);
        int Update(T entity);
        void Update(T entity, IUnitTransaction tran);
        int Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> expression);
        void Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> expression, IUnitTransaction tran);
        List<T> UpdateSelect(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> expression, int top);
        List<TResult> UpdateSelect<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> expression, Expression<Func<T, TResult>> selector, int top);
        IDbConnection Conn { get; }
    }
}

