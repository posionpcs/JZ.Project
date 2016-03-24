namespace Framework.DAL.SqlServer
{
    using Dapper;
    using Microsoft.CSharp.RuntimeBinder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    public class SqlBuilder
    {
        private Dictionary<string, Clauses> data = new Dictionary<string, Clauses>();
        private int seq;

        private void AddClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "", bool IsInclusive = false)
        {
            Clauses clauses;
            if (!this.data.TryGetValue(name, out clauses))
            {
                clauses = new Clauses(joiner, prefix, postfix);
                this.data[name] = clauses;
            }
            Clause item = new Clause {
                Sql = sql,
                Parameters = parameters,
                IsInclusive = IsInclusive
            };
            clauses.Add(item);
            this.seq++;
        }

        public SqlBuilder AddParameters(  object parameters)
        {
            this.AddClause("--parameters", "",  parameters, "");
            return this;
        }

        public Template AddTemplate(string sql,   object parameters = null)
        {
            return null;
            //if (<AddTemplate>o__SiteContainer0.<>p__Site1 == null)
            //{
            //    <AddTemplate>o__SiteContainer0.<>p__Site1 =
            //        CallSite<Func<CallSite, Type, SqlBuilder,
            //        string, object, Template>>.Create
            //        (Binder.InvokeConstructor(CSharpBinderFlags.None, typeof(SqlBuilder),
            //        new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.IsStaticType 
            //            | CSharpArgumentInfoFlags.UseCompileTimeType, null), 
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
            //            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
            //}
            //return <AddTemplate>o__SiteContainer0.<>p__Site1.Target(<AddTemplate>o__SiteContainer0.<>p__Site1, typeof(Template), this, sql, parameters);
        }

        public SqlBuilder GroupBy(string sql,   object parameters = null)
        {
            this.AddClause("GROUPBY", sql,  parameters, " , ", "\nGROUP BY ", "\n");
            return this;
        }

        public SqlBuilder Having(string sql,   object parameters = null)
        {
            this.AddClause("HAVING", sql,  parameters, "\nAND ", "HAVING ", "\n");
            return this;
        }

        public SqlBuilder InnerJoin(string sql,   object parameters = null)
        {
            this.AddClause("INNERJOIN", sql,  parameters, "\nINNER JOIN ", "\nINNER JOIN ", "\n");
            return this;
        }

        public SqlBuilder Intersect(string sql,   object parameters = null)
        {
            this.AddClause("INTERSECT", sql,  parameters, "\nINTERSECT\n ", "\n ", "\n");
            return this;
        }

        public SqlBuilder Join(string sql,   object parameters = null)
        {
            this.AddClause("JOIN", sql,  parameters, "\nJOIN ", "\nJOIN ", "\n");
            return this;
        }

        public SqlBuilder LeftJoin(string sql,   object parameters = null)
        {
            this.AddClause("LEFTJOIN", sql,  parameters, "\nLEFT JOIN ", "\nLEFT JOIN ", "\n");
            return this;
        }

        public SqlBuilder OrderBy(string sql,   object parameters = null)
        {
            this.AddClause("ORDERBY", sql,  parameters, " , ", "ORDER BY ", "\n");
            return this;
        }

        public SqlBuilder OrWhere(string sql,   object parameters = null)
        {
            this.AddClause("WHERE", sql,  parameters, " AND ", "WHERE ", "\n", true);
            return this;
        }

        public SqlBuilder RightJoin(string sql, object parameters = null)
        {
            this.AddClause("RIGHTJOIN", sql,  parameters, "\nRIGHT JOIN ", "\nRIGHT JOIN ", "\n");
            return this;
        }

        public SqlBuilder Select(string sql,   object parameters = null)
        {
            this.AddClause("SELECT", sql,  parameters, " , ", "", "\n");
            return this;
        }

        public SqlBuilder Where(string sql,   object parameters = null)
        {
            this.AddClause("WHERE", sql,  parameters, " AND ", "WHERE ", "\n");
            return this;
        }

        private class Clause
        {
            public bool IsInclusive { get; set; }

            public object Parameters { get; set; }

            public string Sql { get; set; }
        }

        private class Clauses : List<SqlBuilder.Clause>
        {
            private string joiner;
            private string postfix;
            private string prefix;

            public Clauses(string joiner, string prefix = "", string postfix = "")
            {
                this.joiner = joiner;
                this.prefix = prefix;
                this.postfix = postfix;
            }

            public string ResolveClauses(DynamicParameters p)
            {
                foreach (SqlBuilder.Clause clause in this)
                {
                    p.AddDynamicParams(clause.Parameters);
                }
                if (!this.Any<SqlBuilder.Clause>(a => a.IsInclusive))
                {
                    return (this.prefix + string.Join(this.joiner, (IEnumerable<string>) (from c in this select c.Sql)) + this.postfix);
                }
                string[] second = new string[] { " ( " + string.Join(" OR ", (from c in this
                    where c.IsInclusive
                    select c.Sql).ToArray<string>()) + " ) " };
                return (this.prefix + string.Join(this.joiner, (from c in this
                    where !c.IsInclusive
                    select c.Sql).Union<string>(second)) + this.postfix);
            }
        }

        public class Template
        {
            private readonly SqlBuilder builder;
            private int dataSeq = -1;
            private readonly object initParams;
            private object parameters;
            private string rawSql;
            private static Regex regex = new Regex(@"\/\*.+\*\/", RegexOptions.Compiled | RegexOptions.Multiline);
            private readonly string sql;

            public Template(SqlBuilder builder, string sql,   object parameters)
            {
                this.initParams = parameters;
                this.sql = sql;
                this.builder = builder;
            }

            private void ResolveSql()
            {
                if (this.dataSeq != this.builder.seq)
                {
                    DynamicParameters p = new DynamicParameters(this.initParams);
                    this.rawSql = this.sql;
                    foreach (KeyValuePair<string, SqlBuilder.Clauses> pair in this.builder.data)
                    {
                        string oldValue = "/*" + pair.Key + "*/";
                        this.rawSql = this.rawSql.Replace(oldValue, pair.Value.ResolveClauses(p));
                    }
                    this.parameters = p;
                    this.rawSql = regex.Replace(this.rawSql, "");
                    this.dataSeq = this.builder.seq;
                }
            }

            public object Parameters
            {
                get
                {
                    this.ResolveSql();
                    return this.parameters;
                }
            }

            public string RawSql
            {
                get
                {
                    this.ResolveSql();
                    return this.rawSql;
                }
            }
        }
    }
}

