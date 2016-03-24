using System.Collections;
using FrameWork;
using FrameWork.DAL;
using FrameWork.Expressions;
using FrameWork.Utils;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.DAL.SqlServer
{
    public class Repository<T> : IRepository<T>, IDisposable
    {
        //连接字符串
        private IDbConnection conn;
        //是否已经释放
        private bool disposed;
        //是否没有字段
        private static bool isNonKey;
        //实体类的命名空间
        private static readonly string named;
        //工作单元
        private IUnitOfWork uow;
        public IDbConnection Conn
        {
            get
            {
                return this.conn;
            }
        }

        #region 构造函数
        static Repository()
        {
            named = typeof(T).Namespace;
            isNonKey = SqlFormatter<T>.KeyFields.IsNull() || (SqlFormatter<T>.KeyFields.Length == 0);
        }

        public Repository(IUnitOfWork unitOfWork, IDbConnection conn)
        {
            this.uow = unitOfWork;
            this.conn = conn;
        }

        public Repository(IUnitOfWork unitOfWork, Func<string, IDbConnection> aquire)
        {
            this.uow = unitOfWork;
            this.conn = aquire(named);
        } 
        #endregion

        #region 新增
        public int Add(T entity)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildAddCommand(entity);
            return this.dbAdd(entity, cmd, null);
        }

        public int Add(IEnumerable<T> entities)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildAddCommand(entities);
            return this.dbAdd(entities, cmd, null);
        }

        public void Add(T entity, IUnitTransaction tran)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildAddCommand(entity);
            ((UnitTransaction)tran).Register(t => ((Repository<T>)this).dbAdd(entity, cmd, t), this.conn);
        }

        public void Add(IEnumerable<T> entities, IUnitTransaction tran)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildAddCommand(entities);
            ((UnitTransaction)tran).Register(t => ((Repository<T>)this).conn.Execute(cmd.Sql, cmd.Parameters, t, null, null), this.conn);
        }

        public int AddIfNotExists(T entity, Expression<Func<T, bool>> predicate)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildAddCommand(entity, predicate);
            return this.dbAddIfNotExists(entity, cmd, null);
        }

        public void AddIfNotExists(T entity, Expression<Func<T, bool>> predicate, IUnitTransaction tran)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildAddCommand(entity, predicate);
            ((UnitTransaction)tran).Register(t => ((Repository<T>)this).dbAddIfNotExists(entity, cmd, t), this.conn);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="timeout"></param>
        public void BatchInsert(IEnumerable<T> entities, int timeout = 60)
        {
            DataTable table = new DataTable();
            foreach (PropertyInfo info in SqlFormatter<T>.Fields)
            {
                if (TypeHelper.IsNullableType(info.PropertyType))
                {
                    table.Columns.Add(info.Name, TypeHelper.GetNonNullableType(info.PropertyType));
                }
                else
                {
                    table.Columns.Add(info.Name, info.PropertyType);
                }
            }
            foreach (T local in entities)
            {
                DataRow row = table.NewRow();
                foreach (PropertyInfo info2 in SqlFormatter<T>.Fields)
                {
                    row[info2.Name] = MemberAccessor.Process(local, info2) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            bool flag = false;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                flag = true;
            }
            try
            {
                using (SqlBulkCopy copy = new SqlBulkCopy((SqlConnection)conn))
                {
                    copy.BatchSize = entities.Count<T>();
                    copy.BulkCopyTimeout = timeout;
                    copy.DestinationTableName = typeof(T).Name;
                    foreach (PropertyInfo info3 in SqlFormatter<T>.Fields)
                    {
                        copy.ColumnMappings.Add(info3.Name, info3.Name);
                    }
                    copy.WriteToServer(table);
                }
            }
            finally
            {
                if (flag)
                {
                    conn.Close();
                }
            }
        } 
        #endregion

        public int Count(Expression<Func<T, bool>> predicate, string dbLock = "")
        {
            SqlCmd cmd = SqlFormatter<T>.BuildCountCommand(predicate, dbLock);
            return conn.ExecuteScalar<int>(cmd.Sql, cmd.Parameters, null, null, null);
        }

        private int dbAdd(T entity, SqlCmd cmd, IDbTransaction tran)
        {
            PropertyInfo autoIncrement = SqlFormatter<T>.AutoIncrement;
            if (!autoIncrement.IsNull())
            {
                object obj2 = conn.ExecuteScalar(cmd.Sql, cmd.Parameters, tran, null, null);
                autoIncrement.SetValue(entity, Convert.ChangeType(obj2, autoIncrement.PropertyType));
                return 1;
            }
            return conn.Execute(cmd.Sql, cmd.Parameters, tran, null, null);
        }

        private int dbAdd(IEnumerable<T> entities, SqlCmd cmd, IDbTransaction tran)
        {
            return conn.Execute(cmd.Sql, cmd.Parameters, tran, null, null);
        }

        public int dbAddIfNotExists(T entity, SqlCmd cmd, IDbTransaction tran)
        {
            object obj2 = conn.ExecuteScalar(cmd.Sql, cmd.Parameters, tran, null, null);
            PropertyInfo autoIncrement = SqlFormatter<T>.AutoIncrement;
            if (autoIncrement.IsNull())
            {
                return (int) obj2;
            }
            if (obj2 == null)
            {
                return 0;
            }
            autoIncrement.SetValue(entity, Convert.ChangeType(obj2, autoIncrement.PropertyType));
            return 1;
        }

        #region Dapper提供的方法封装
        public int dbDelete(SqlCmd cmd, IDbTransaction tran)
        {
            return conn.Execute(cmd.Sql, cmd.Parameters, tran, null, null);
        }

        public int dbExecute(string sql, object parms, IDbTransaction tran)
        {
            return conn.Execute(sql, parms, tran, null, null);
        }

        private int dbUpdate(SqlCmd cmd, IDbTransaction tran)
        {
            return conn.Execute(cmd.Sql, cmd.Parameters, tran, null, null);
        }
        #endregion
        
        #region 删除
        public int Delete(T entity)
        {
            ThrowIfNonKeys();
            SqlCmd cmd = SqlFormatter<T>.BuildDeleteCommand(entity);
            return dbDelete(cmd, null);
        }
        public int Delete(Expression<Func<T, bool>> predicate)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildDeleteCommand(predicate);
            return dbDelete(cmd, null);
        }
        public void Delete(T entity, IUnitTransaction tran)
        {
            ThrowIfNonKeys();
            SqlCmd cmd = SqlFormatter<T>.BuildDeleteCommand(entity);
            ((UnitTransaction)tran).Register(t => dbDelete(cmd, t), conn);
        }
        public void Delete(Expression<Func<T, bool>> predicate, IUnitTransaction tran)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildDeleteCommand(predicate);
            ((UnitTransaction)tran).Register(t => dbDelete(cmd, t),conn);
        } 
        #endregion

        #region 内存释放
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ((!this.disposed && disposing) && (this.conn != null))
            {
                this.conn.Dispose();
            }
            this.disposed = true;
        } 
        #endregion

        public int Execute(string sql, object parms)
        {
            return dbExecute(sql, parms, null);
        }

        public void Execute(string sql, object parms, IUnitTransaction tran)
        {
            ((UnitTransaction)tran).Register(dbtrans =>dbExecute(sql, parms, dbtrans), conn);
        }

        public TResult ExecuteScalar<TResult>(string sql, object parms)
        {
            return conn.ExecuteScalar<TResult>(sql, parms, null, null, null);
        }
       
        public IEnumerable<TResult> ExecuteStoredProcedure<TResult>(string procedure, object parms)
        {
            return conn.Query<TResult>(procedure, parms, null,true,4);
        }

        public bool Exists(Expression<Func<T, bool>> predicate, string dbLock = "")
        {
            return (Count(predicate, dbLock) > 0);
        }


        #region Get

        public T Get(Expression<Func<T, bool>> predicate, string dbLock = "")
        {
            return GetEx<T>(predicate, null, null, dbLock);
        }

        public T Get(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "")
        {
            return GetEx<T>(predicate, null, orderby, dbLock);
        }

        public TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return GetEx(predicate, selector, null, "");
        }

        public TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby)
        {
            return GetEx(predicate, selector, orderby, "");
        }

        public TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string dbLock)
        {
            return GetEx(predicate, selector, null, dbLock);
        }

        public TResult GetEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, string dbLock)
        {
            string str = (orderby == null) ? string.Empty : (orderby(new DbSort<T>()));
            SqlCmd cmd = SqlFormatter<T>.BuildGetCommand(predicate, selector, str, dbLock);
            return conn.Query<TResult>(cmd.Sql, cmd.Parameters, null, true, null, null).SingleOrDefault<TResult>();
        }
        
        #endregion

        #region 获取分页数据
        private PageList<TResult> GetPageList<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, int pageIndex, int pageSize, string dbLock)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildPageCommand<TResult>(predicate, selector, orderby, pageIndex, pageSize, dbLock);
            using (SqlMapper.GridReader reader = this.conn.QueryMultiple(cmd.Sql, cmd.Parameters, null, null, null))
            {
                return new PageList<TResult>(reader.Read<int>(true).First<int>(), pageSize, pageIndex, reader.Read<TResult>(true).ToList<TResult>());
            }
        }

        public PageList<T> PageList(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize)
        {
            return this.PageListEx<T>(predicate, null, orderby, pageIndex, pageSize, "(NOLOCK)");
        }

        public PageList<T> PageList(Expression<Func<T, bool>> predicate, string orderby, int pageIndex, int pageSize)
        {
            return this.PageListEx<T>(predicate, null, orderby, pageIndex, pageSize, "(NOLOCK)");
        }

        public PageList<T> PageList(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize, string dbLock)
        {
            return this.PageListEx<T>(predicate, null, orderby, pageIndex, pageSize, dbLock);
        }

        public PageList<T> PageList(Expression<Func<T, bool>> predicate, string orderby, int pageIndex, int pageSize, string dbLock)
        {
            return this.PageListEx<T>(predicate, null, orderby, pageIndex, pageSize, dbLock);
        }

        public PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize)
        {
            return this.PageListEx<TResult>(predicate, selector, orderby, pageIndex, pageSize, "(NOLOCK)");
        }

        public PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, int pageIndex, int pageSize)
        {
            return this.PageListEx<TResult>(predicate, selector, orderby, pageIndex, pageSize, "(NOLOCK)");
        }

        public PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, int pageIndex, int pageSize, string dbLock)
        {
            return this.GetPageList<TResult>(predicate, selector, orderby(new DbSort<T>(null)), pageIndex, pageSize, dbLock);
        }

        public PageList<TResult> PageListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, int pageIndex, int pageSize, string dbLock)
        {
            return this.GetPageList<TResult>(predicate, selector, orderby, pageIndex, pageSize, dbLock);
        }
        
        #endregion

        #region Query
        public IEnumerable<TResult> Query<TResult>(string sql, object parms)
        {
            return this.conn.Query<TResult>(sql, parms, null, true, null, null);
        }

        public List<IEnumerable> Query<T1, T2>(string sql, object parms)
        {
            List<IEnumerable> list = new List<IEnumerable>();
            using (SqlMapper.GridReader reader = conn.QueryMultiple(sql, parms, null, null, null))
            {
                list.Add(reader.Read<T1>(true));
                list.Add(reader.Read<T2>(true));
                return list;
            }
        }
        public List<IEnumerable> Query<T1, T2, T3>(string sql, object parms)
        {
            List<IEnumerable> list = new List<IEnumerable>();
            using (SqlMapper.GridReader reader = this.conn.QueryMultiple(sql, parms, null, null, null))
            {
                list.Add(reader.Read<T1>(true));
                list.Add(reader.Read<T2>(true));
                list.Add(reader.Read<T3>(true));
                return list;
            }
        }

        public List<IEnumerable> Query<T1, T2, T3, T4>(string sql, object parms)
        {
            List<IEnumerable> list = new List<IEnumerable>();
            using (SqlMapper.GridReader reader = this.conn.QueryMultiple(sql, parms, null, null, null))
            {
                list.Add(reader.Read<T1>(true));
                list.Add(reader.Read<T2>(true));
                list.Add(reader.Read<T3>(true));
                list.Add(reader.Read<T4>(true));
                return list;
            }
        }

        public List<IEnumerable> Query<T1, T2, T3, T4, T5>(string sql, object parms)
        {
            List<IEnumerable> list = new List<IEnumerable>();
            using (SqlMapper.GridReader reader = conn.QueryMultiple(sql, parms, null, null, null))
            {
                list.Add(reader.Read<T1>(true));
                list.Add(reader.Read<T2>(true));
                list.Add(reader.Read<T3>(true));
                list.Add(reader.Read<T4>(true));
                list.Add(reader.Read<T5>(true));
                return list;
            }
        } 
        #endregion

        private void ThrowIfNonKeys()
        {
            if (isNonKey)
            {
                throw new Exception("{0}未设置主键");
            }
        }

        #region 获取集合
        public List<T> ToList(Expression<Func<T, bool>> predicate, string dbLock = "")
        {
            return ToListEx<T>(predicate, null, 0, null, dbLock);
        }

        public List<T> ToList(Expression<Func<T, bool>> predicate, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "")
        {
            return ToListEx<T>(predicate, null, 0, orderby, dbLock);
        }

        public List<T> ToList(Expression<Func<T, bool>> predicate, int top, string dbLock = "")
        {
            return ToListEx<T>(predicate, null, top, null, dbLock);
        }

        public List<T> ToList(Expression<Func<T, bool>> predicate, int top, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "")
        {
            return ToListEx<T>(predicate, null, top, orderby, dbLock);
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return ToListEx(predicate, selector, 0, null, "");
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby)
        {
            return ToListEx(predicate, selector, 0, orderby, "");
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top)
        {
            return ToListEx(predicate, selector, top, null, "");
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string dbLock = "")
        {
            return ToListEx(predicate, selector, 0, null, dbLock);
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "")
        {
            return ToListEx(predicate, selector, 0, orderby, dbLock);
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top, Func<DbSort<T>, DbSort<T>> orderby)
        {
            string str = orderby.IsNull() ? string.Empty : (orderby(new DbSort<T>()));
            SqlCmd cmd = SqlFormatter<T>.BuildListCommand(predicate, selector, str,top, "");
            return this.conn.Query<TResult>(cmd.Sql, cmd.Parameters, null, true, null, null).ToList<TResult>();
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top, string dbLock = "")
        {
            return ToListEx(predicate, selector, top, null, dbLock);
        }

        public List<TResult> ToListEx<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int top, Func<DbSort<T>, DbSort<T>> orderby, string dbLock = "")
        {
            string str = orderby.IsNull() ? string.Empty : orderby(new DbSort<T>());
            SqlCmd cmd = SqlFormatter<T>.BuildListCommand(predicate, selector, str, (top), dbLock);
            return conn.Query<TResult>(cmd.Sql, cmd.Parameters, null, true, null, null).ToList<TResult>();
        } 
        #endregion

        #region 编辑
        public int Update(T entity)
        {
            ThrowIfNonKeys();
            SqlCmd cmd = SqlFormatter<T>.BuildUpdateCommand(entity);
            return dbUpdate(cmd, null);
        }

        public void Update(T entity, IUnitTransaction tran)
        {
            ThrowIfNonKeys();
            SqlCmd cmd = SqlFormatter<T>.BuildUpdateCommand(entity);
            ((UnitTransaction)tran).Register(t =>dbUpdate(cmd, t), conn);
        }

        public int Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> expression)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildUpdateCommand(predicate, expression);
            return dbUpdate(cmd, null);
        }

        public void Update(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> expression, IUnitTransaction tran)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildUpdateCommand(predicate, expression);
            ((UnitTransaction)tran).Register(t =>dbUpdate(cmd, t), conn);
        }

        public List<T> UpdateSelect(Expression<Func<T, bool>> predicate,
            Expression<Func<T, T>> expression,
            int top)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildUpdateSelectCommand<T>(predicate, expression, null, top);
            return conn.Query<T>(cmd.Sql, cmd.Parameters, null, true, null, null).ToList<T>();
        }

        public List<TResult> UpdateSelect<TResult>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, T>> expression,
            Expression<Func<T, TResult>> selector,
            int top)
        {
            SqlCmd cmd = SqlFormatter<T>.BuildUpdateSelectCommand(predicate, expression, selector, top);
            return conn.Query<TResult>(cmd.Sql, cmd.Parameters, null, true, null, null).ToList<TResult>();
        } 
        #endregion
    }
}

