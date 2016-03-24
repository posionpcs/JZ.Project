using System.ComponentModel.DataAnnotations.Schema;
using FrameWork;
using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Framework.DAL.SqlServer
{
    public class SqlFormatter<T>
    {
        private static string _table;
        public static PropertyInfo AutoIncrement;
        private static string DeleteSql;
        public static PropertyInfo[] Fields;
        private static string InsertNotExistsSql;
        private static string InsertSql;
        public static PropertyInfo[] KeyFields;
        public static string KeyFieldString;
        public static PropertyInfo[] NoIncFields;
        public static string NoKeyFieldString;
        public static string SelectFieldString;
        private static string UpdateSql;

        static SqlFormatter()
        {
             _table = string.Format("[{0}]", typeof(T).Name);
             InitFields();
             InitFieldString();
             InitInsertSql();
             InitInsertNotExistsSql();
             InitUpdateSql();
             InitDeleteSql();
        }

        private static void addIf(StringBuilder builder, SqlCmd cmd)
        {
            if (!string.IsNullOrEmpty(cmd.Sql))
            {
                builder.Append(" WHERE ");
                builder.Append(cmd.Sql);
                builder.Append(" ");
            }
        }

        public static SqlCmd BuildAddCommand(T entity)
        {
            DynamicParameters parameters = new DynamicParameters();
            foreach (PropertyInfo info in  NoIncFields)
            {
                object obj2 = info.GetValue(entity);
                DbType? dbType = null;
                ParameterDirection? direction = null;
                int? size = null;
                byte? precision = null;
                byte? scale = null;
                parameters.Add(info.Name, obj2, dbType, direction, size, precision, scale);
            }
            return new SqlCmd { Sql =  InsertSql, Parameters = parameters };
        }

        public static SqlCmd BuildAddCommand(IEnumerable<T> entities)
        {
            DynamicParameters parameters = new DynamicParameters();
            int num = 0;
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append( _table);
            builder.Append(" (");
            bool flag = false;
            foreach (PropertyInfo info in  NoIncFields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append(info.Name);
                flag = true;
            }
            builder.Append(") VALUES ");
            bool flag2 = false;
            foreach (T local in entities)
            {
                if (flag2)
                {
                    builder.Append(",");
                }
                builder.Append("(");
                flag = false;
                foreach (PropertyInfo info2 in  NoIncFields)
                {
                    string str = "p" + num++;
                    if (flag)
                    {
                        builder.Append(",");
                    }
                    builder.Append("@");
                    builder.Append(str);
                    object obj2 = info2.GetValue(local);
                    DbType? dbType = null;
                    ParameterDirection? direction = null;
                    int? size = null;
                    byte? precision = null;
                    byte? scale = null;
                    parameters.Add(str, obj2, dbType, direction, size, precision, scale);
                    flag = true;
                }
                builder.Append(")");
                flag2 = true;
            }
            return new SqlCmd { Sql = builder.ToString(), Parameters = parameters };
        }

        public static SqlCmd BuildAddCommand(T entity, Expression<Func<T, bool>> predicate)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append( InsertNotExistsSql);
            builder.Append("WHERE NOT EXISTS(SELECT TOP 1 * FROM ");
            builder.Append( _table);
            SqlCmd cmd = new PredicateReader().Translate(predicate);
             addIf(builder, cmd);
            builder.Append(")");
            if ( AutoIncrement != null)
            {
                builder.Append(";IF(@@ROWCOUNT > 0) BEGIN\tSELECT SCOPE_IDENTITY(); END ELSE BEGIN SELECT NULL; END;");
            }
            else
            {
                builder.Append("; SELECT @@ROWCOUNT;");
            }
            cmd.Sql = builder.ToString();
            foreach (PropertyInfo info in  NoIncFields)
            {
                object obj2 = info.GetValue(entity);
                DbType? dbType = null;
                ParameterDirection? direction = null;
                int? size = null;
                byte? precision = null;
                byte? scale = null;
                cmd.Parameters.Add(info.Name, obj2, dbType, direction, size, precision, scale);
            }
            return cmd;
        }

        public static SqlCmd BuildCountCommand(Expression<Func<T, bool>> predicate, string dbLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COUNT(1) FROM ");
            builder.Append( _table);
            builder.Append(dbLock);
            SqlCmd cmd = new PredicateReader().Translate(predicate);
             addIf(builder, cmd);
            cmd.Sql = builder.ToString();
            return cmd;
        }

        public static SqlCmd BuildDeleteCommand(T entity)
        {
            DynamicParameters parameters = new DynamicParameters();
            foreach (PropertyInfo info in  KeyFields)
            {
                object obj2 = info.GetValue(entity);
                DbType? dbType = null;
                ParameterDirection? direction = null;
                int? size = null;
                byte? precision = null;
                byte? scale = null;
                parameters.Add(info.Name, obj2, dbType, direction, size, precision, scale);
            }
            return new SqlCmd { Sql =  DeleteSql, Parameters = parameters };
        }

        public static SqlCmd BuildDeleteCommand(Expression<Func<T, bool>> predicate)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append( _table);
            SqlCmd cmd = new PredicateReader().Translate(predicate);
             addIf(builder, cmd);
            cmd.Sql = builder.ToString();
            return cmd;
        }

        public static SqlCmd BuildGetCommand<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, string dbLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT TOP 1 ");
            if (selector == null)
            {
                builder.Append("*");
            }
            else
            {
                 ReadSelector<TResult>(builder, selector, false);
            }
            builder.Append(" FROM ");
            builder.Append( _table);
            builder.Append(dbLock);
            SqlCmd cmd = new PredicateReader().Translate(predicate);
             addIf(builder, cmd);
            if (!orderby.IsNullOrEmpty())
            {
                builder.Append(" ORDER BY ");
                builder.Append(orderby);
            }
            cmd.Sql = builder.ToString();
            return cmd;
        }

        public static SqlCmd BuildListCommand<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, int? top, string dbLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            if (top > 0)
            {
                builder.AppendFormat("TOP {0} ", top);
            }
            if (selector.IsNull<Expression<Func<T, TResult>>>())
            {
                builder.Append("*");
            }
            else
            {
                 ReadSelector<TResult>(builder, selector, false);
            }
            builder.Append(" FROM ");
            builder.Append( _table);
            builder.Append(dbLock);
            SqlCmd cmd = new PredicateReader().Translate(predicate);
             addIf(builder, cmd);
            if (!orderby.IsNullOrWhiteSpace())
            {
                builder.Append(" ORDER BY ");
                builder.Append(orderby);
            }
            cmd.Sql = builder.ToString();
            return cmd;
        }

        public static SqlCmd BuildPageCommand<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, string orderby, int pageIndex, int pageSize, string dbLock)
        {
            StringBuilder builder = new StringBuilder();
            SqlCmd cmd = new PredicateReader().Translate(predicate);
            builder.Append("SELECT COUNT(1) FROM ");
            builder.Append( _table);
            builder.Append(dbLock);
             addIf(builder, cmd);
            builder.Append(";");
            builder.Append("SELECT ");
            if (selector == null)
            {
                builder.Append("*");
            }
            else
            {
                 ReadSelector<TResult>(builder, selector, false);
            }
            builder.Append(" FROM(");
            builder.Append("SELECT ");
            if (selector == null)
            {
                builder.Append("*");
            }
            else
            {
                 ReadSelector<TResult>(builder, selector, false);
            }
            builder.AppendFormat(",ROW_NUMBER() OVER(ORDER BY {0}) AS RowNumber ", orderby);
            builder.AppendFormat("FROM {0}{1} A",  _table, dbLock);
             addIf(builder, cmd);
            builder.Append(" ) B");
            builder.AppendFormat(" WHERE RowNumber > {0} AND RowNumber <= {1}", (pageIndex - 1) * pageSize, pageIndex * pageSize);
            cmd.Sql = builder.ToString();
            return cmd;
        }

        public static SqlCmd BuildUpdateCommand(T entity)
        {
            DynamicParameters parameters = new DynamicParameters();
            foreach (PropertyInfo info in  Fields)
            {
                object obj2 = info.GetValue(entity);
                DbType? dbType = null;
                ParameterDirection? direction = null;
                int? size = null;
                byte? precision = null;
                byte? scale = null;
                parameters.Add(info.Name, obj2, dbType, direction, size, precision, scale);
            }
            return new SqlCmd { Sql =  UpdateSql, Parameters = parameters };
        }

        public static SqlCmd BuildUpdateCommand(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression)
        {
            MemberInitExpression body = updateExpression.Body as MemberInitExpression;
            body.ThrowIf<MemberInitExpression>(m => m == null, "The updateExpression must be of type MemberInitExpression.");
            SqlCmd cmd = new PredicateReader().Translate(predicate);
            StringBuilder builder = new StringBuilder();
            body = PartialEvaluator.Eval<T>(body) as MemberInitExpression;
            builder.Append("UPDATE ");
            builder.Append( _table);
            builder.Append(" SET ");
            bool flag = false;
            foreach (MemberBinding binding in body.Bindings)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                ConstantExpression expression = (binding as MemberAssignment).Expression as ConstantExpression;
                builder.Append(binding.Member.Name);
                builder.Append(" = @");
                builder.Append(binding.Member.Name);
                DbType? dbType = null;
                ParameterDirection? direction = null;
                int? size = null;
                byte? precision = null;
                byte? scale = null;
                cmd.Parameters.Add("@" + binding.Member.Name, expression.Value, dbType, direction, size, precision, scale);
                flag = true;
            }
             addIf(builder, cmd);
            cmd.Sql = builder.ToString();
            return cmd;
        }

        public static SqlCmd BuildUpdateSelectCommand<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression, Expression<Func<T, TResult>> selector, int top)
        {
            MemberInitExpression body = updateExpression.Body as MemberInitExpression;
            body.ThrowIf<MemberInitExpression>(m => m == null, "The updateExpression must be of type MemberInitExpression.");
            SqlCmd cmd = new PredicateReader().Translate(predicate);
            StringBuilder builder = new StringBuilder();
            body = PartialEvaluator.Eval<T>(body) as MemberInitExpression;
            builder.Append("UPDATE TOP(");
            builder.Append(top);
            builder.Append(") ");
            builder.Append( _table);
            builder.Append(" WITH (UPDLOCK, READPAST) SET ");
            bool flag = false;
            foreach (MemberBinding binding in body.Bindings)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                ConstantExpression expression = (binding as MemberAssignment).Expression as ConstantExpression;
                builder.Append(binding.Member.Name);
                builder.Append(" = @");
                builder.Append(binding.Member.Name);
                DbType? dbType = null;
                ParameterDirection? direction = null;
                int? size = null;
                byte? precision = null;
                byte? scale = null;
                cmd.Parameters.Add("@" + binding.Member.Name, expression.Value, dbType, direction, size, precision, scale);
                flag = true;
            }
            builder.Append(" OUTPUT ");
            if (selector.IsNull<Expression<Func<T, TResult>>>())
            {
                builder.Append("INSERTED.*");
            }
            else
            {
                 ReadSelector<TResult>(builder, selector, true);
            }
             addIf(builder, cmd);
            cmd.Sql = builder.ToString();
            return cmd;
        }

        public static string GetCountSql(string dbLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT COUNT(1) FROM ");
            builder.Append( _table);
            builder.Append(dbLock);
            return builder.ToString();
        }

        public static string GetSelectSql(int top, string orderby, string dbLock)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            if (top > 0)
            {
                builder.AppendFormat("TOP {0} ", top);
            }
            builder.Append("*");
            builder.Append(" FROM ");
            builder.Append( _table);
            builder.Append(dbLock);
            builder.Append(" ");
            builder.Append(orderby);
            return builder.ToString();
        }

        private static void InitDeleteSql()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DELETE FROM ");
            builder.Append( _table);
            builder.Append(" WHERE ");
            bool flag = false;
            foreach (PropertyInfo info in  KeyFields)
            {
                if (flag)
                {
                    builder.Append(" AND ");
                }
                builder.Append(info.Name);
                builder.Append("=");
                builder.Append("@");
                builder.Append(info.Name);
                flag = true;
            }
             DeleteSql = builder.ToString();
        }

        private static void InitFields()
        {
             Fields = typeof(T).GetProperties();
             KeyFields = (from f in  Fields
                where Attribute.IsDefined(f, typeof(KeyAttribute))
                select f).ToArray<PropertyInfo>();
             AutoIncrement = (from f in  Fields
                where Attribute.IsDefined(f, typeof(DatabaseGeneratedAttribute))
                select f).SingleOrDefault<PropertyInfo>();
            if ( AutoIncrement != null)
            {
                 NoIncFields = (from f in  Fields
                    where f.Name !=  AutoIncrement.Name
                    select f).ToArray<PropertyInfo>();
            }
            else
            {
                 NoIncFields =  Fields;
            }
        }

        private static void InitFieldString()
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            foreach (PropertyInfo info in  Fields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append(info.Name);
                flag = true;
            }
             SelectFieldString = builder.ToString();
            builder.Clear();
            flag = false;
            foreach (PropertyInfo info2 in  NoIncFields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append(info2.Name);
                flag = true;
            }
             NoKeyFieldString = builder.ToString();
            builder.Clear();
            flag = false;
            foreach (PropertyInfo info3 in  KeyFields)
            {
                if (flag)
                {
                    builder.Append(" AND");
                }
                builder.Append(info3.Name);
                builder.Append("=");
                builder.Append("@");
                builder.Append(info3.Name);
                flag = true;
            }
             KeyFieldString = builder.ToString();
        }

        private static void InitInsertNotExistsSql()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append( _table);
            builder.Append(" (");
            bool flag = false;
            foreach (PropertyInfo info in  NoIncFields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append(info.Name);
                flag = true;
            }
            builder.Append(") SELECT ");
            flag = false;
            foreach (PropertyInfo info2 in  NoIncFields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append("@");
                builder.Append(info2.Name);
                flag = true;
            }
            builder.Append(" ");
             InsertNotExistsSql = builder.ToString();
        }

        private static void InitInsertSql()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("INSERT INTO ");
            builder.Append( _table);
            builder.Append(" (");
            bool flag = false;
            foreach (PropertyInfo info in  NoIncFields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append(info.Name);
                flag = true;
            }
            builder.Append(") VALUES (");
            flag = false;
            foreach (PropertyInfo info2 in  NoIncFields)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append("@");
                builder.Append(info2.Name);
                flag = true;
            }
            builder.Append(")");
            if ( AutoIncrement != null)
            {
                builder.Append(";SELECT SCOPE_IDENTITY();");
            }
             InsertSql = builder.ToString();
        }

        private static void InitUpdateSql()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("UPDATE ");
            builder.Append( _table);
            builder.Append(" SET ");
            bool flag = false;
            foreach (PropertyInfo info in  NoIncFields.Except<PropertyInfo>( KeyFields))
            {
                if (flag)
                {
                    builder.Append(",");
                }
                builder.Append(info.Name);
                builder.Append("=");
                builder.Append("@");
                builder.Append(info.Name);
                flag = true;
            }
            builder.Append(" WHERE ");
            flag = false;
            foreach (PropertyInfo info2 in  KeyFields)
            {
                if (flag)
                {
                    builder.Append(" AND ");
                }
                builder.Append(info2.Name);
                builder.Append("=");
                builder.Append("@");
                builder.Append(info2.Name);
                flag = true;
            }
             UpdateSql = builder.ToString();
        }

        private static void ReadSelector<TResult>(StringBuilder builder, Expression<Func<T, TResult>> selector, bool inserted = false)
        {
            MemberInitExpression body = selector.Body as MemberInitExpression;
            if (body != null)
            {
                ReadOnlyCollection<MemberBinding> bindings = body.Bindings;
                bool flag = false;
                foreach (MemberBinding binding in bindings)
                {
                    MemberExpression expression = (binding as MemberAssignment).Expression as MemberExpression;
                    if (flag)
                    {
                        builder.Append(",");
                    }
                    if (inserted)
                    {
                        builder.Append("INSERTED.");
                    }
                    builder.Append(expression.Member.Name);
                    builder.Append(" AS ");
                    builder.Append(binding.Member.Name);
                    flag = true;
                }
            }
            else
            {
                MemberExpression expression3 = selector.Body as MemberExpression;
                if (expression3 != null)
                {
                    if (inserted)
                    {
                        builder.Append("INSERTED.");
                    }
                    builder.Append(expression3.Member.Name);
                }
                else
                {
                    builder.Append(inserted ? DynamicEntity<TResult>.InsertedSqlFields : DynamicEntity<TResult>.SqlFields);
                }
            }
        }
    }
}

