using System;
using System.Linq.Expressions;

namespace FrameWork.DAL
{
    public class DbSort<T>
    {
        private string orderby;

        public DbSort(string str = null)
        {
            this.orderby = str;
        }

        public DbSort<T> Asc<TField>(Expression<Func<T, TField>> expression)
        {
            this.orderby = this.orderby + "," + (expression.Body as MemberExpression).Member.Name;
            return (DbSort<T>) this;
        }

        public DbSort<T> Desc<TField>(Expression<Func<T, TField>> expression)
        {
            this.orderby = this.orderby + "," + (expression.Body as MemberExpression).Member.Name + " DESC";
            return (DbSort<T>) this;
        }

        public static implicit operator string(DbSort<T> value)
        {
            if (value != null)
            {
                return value.orderby.TrimStart(new char[] { ',' });
            }
            return null;
        }
    }
}

