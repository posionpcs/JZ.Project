using System.Data;

namespace Framework.DAL.SqlServer
{
    using Dapper;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public class PredicateReader : ExpressionVisitor
    {
        private StringBuilder builder;
        private SqlCmd cmd;
        private int index;

        public SqlCmd Translate(LambdaExpression expression)
        {
            this.index = 0;
            this.cmd = new SqlCmd();
            Expression node = PartialEvaluator.Eval(expression);
            SqlCmd cmd = this.TryIf(expression);
            if (cmd != null)
            {
                return cmd;
            }
            this.builder = new StringBuilder();
            this.Visit(node);
            this.cmd.Sql = this.builder.ToString();
            return this.cmd;
        }

        private SqlCmd TryIf(LambdaExpression expression)
        {
            if ((expression.Body.NodeType != ExpressionType.Constant) || !(expression.Body.Type == typeof(bool)))
            {
                return null;
            }
            if ((bool) ((ConstantExpression) expression.Body).Value)
            {
                return new SqlCmd { Sql = null, Parameters = new DynamicParameters() };
            }
            return new SqlCmd { Sql = "1=0", Parameters = new DynamicParameters() };
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.AndAlso)
            {
                if ((node.Left.NodeType == ExpressionType.Constant) && (node.Left.Type == typeof(bool)))
                {
                    if ((bool) ((ConstantExpression) node.Left).Value)
                    {
                        this.Visit(node.Right);
                        return node;
                    }
                    this.builder.Append("1=0");
                    return node;
                }
                if ((node.Right.NodeType == ExpressionType.Constant) && (node.Right.Type == typeof(bool)))
                {
                    if ((bool) ((ConstantExpression) node.Right).Value)
                    {
                        this.Visit(node.Left);
                        return node;
                    }
                    this.builder.Append("1=0");
                    return node;
                }
            }
            if (node.NodeType == ExpressionType.OrElse)
            {
                if ((node.Left.NodeType == ExpressionType.Constant) && (node.Left.Type == typeof(bool)))
                {
                    if (!((bool) ((ConstantExpression) node.Left).Value))
                    {
                        this.Visit(node.Right);
                    }
                    return node;
                }
                if ((node.Right.NodeType == ExpressionType.Constant) && (node.Right.Type == typeof(bool)))
                {
                    if (!((bool) ((ConstantExpression) node.Right).Value))
                    {
                        this.Visit(node.Left);
                    }
                    return node;
                }
            }
            this.builder.Append("(");
            this.Visit(node.Left);
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    if ((node.Right.NodeType != ExpressionType.Constant) || (((ConstantExpression) node.Right).Value != null))
                    {
                        this.builder.Append(" = ");
                    }
                    else
                    {
                        this.builder.Append(" IS ");
                    }
                    break;

                case ExpressionType.GreaterThan:
                    this.builder.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    this.builder.Append(" >= ");
                    break;

                case ExpressionType.LessThan:
                    this.builder.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    this.builder.Append(" <= ");
                    break;

                case ExpressionType.AndAlso:
                    this.builder.Append(" AND ");
                    break;

                case ExpressionType.NotEqual:
                    if ((node.Right.NodeType != ExpressionType.Constant) || (((ConstantExpression) node.Right).Value != null))
                    {
                        this.builder.Append(" <> ");
                    }
                    else
                    {
                        this.builder.Append(" IS NOT ");
                    }
                    break;

                case ExpressionType.OrElse:
                    this.builder.Append(" OR ");
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported.", node.NodeType));
            }
            this.Visit(node.Right);
            this.builder.Append(")");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                this.builder.Append("NULL");
                return node;
            }
            TypeCode typeCode = Type.GetTypeCode(node.Value.GetType());
            object obj2 = (typeCode == TypeCode.Boolean) ? (((bool) node.Value) ? 1 : 0) : node.Value;
            switch (typeCode)
            {
                case TypeCode.Object:
                    throw new NotSupportedException(string.Format("The constant for '{0}' is not supported.", node.Value));
            }
            string str = "@P" + this.index++;
            this.builder.Append(str);
            this.cmd.Parameters.Add(str, obj2, null, null, null, null, null);
            return node;
        }

        private MethodCallExpression VisitContains(MethodCallExpression node, ConstantExpression source, MemberExpression member)
        {
            bool flag1 = member.Type == typeof(string);
            this.builder.Append(member.Member.Name);
            this.builder.Append(" IN (");
            bool flag = false;
            foreach (object obj2 in (IEnumerable) source.Value)
            {
                if (flag)
                {
                    this.builder.Append(",");
                }
                string str = "@P" + this.index++;
                this.builder.Append(str);
                DbType? dbType = null;
                ParameterDirection? direction = null;
                int? size = null;
                byte? precision = null;
                byte? scale = null;
                this.cmd.Parameters.Add(str, obj2, dbType, direction, size, precision, scale);
                flag = true;
            }
            this.builder.Append(")");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.builder.Append(node.Member.Name);
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "StartsWith":
                    if (node.Method.DeclaringType == typeof(string))
                    {
                        MemberExpression expression = node.Object as MemberExpression;
                        this.builder.Append(expression.Member.Name);
                        string str = "@P" + this.index++;
                        this.builder.AppendFormat(" LIKE ", new object[0]);
                        this.builder.Append(str);
                        object obj2 = (node.Arguments[0] as ConstantExpression).Value;
                        this.cmd.Parameters.Add(str, obj2 + "%", null, null, null, null, null);
                    }
                    return node;

                case "EndsWith":
                    if (node.Method.DeclaringType == typeof(string))
                    {
                        MemberExpression expression2 = node.Object as MemberExpression;
                        this.builder.Append(expression2.Member.Name);
                        string str2 = "@P" + this.index++;
                        this.builder.AppendFormat(" LIKE ", new object[0]);
                        this.builder.Append(str2);
                        object obj3 = (node.Arguments[0] as ConstantExpression).Value;
                        this.cmd.Parameters.Add(str2, "%" + obj3, null, null, null, null, null);
                    }
                    return node;

                case "Contains":
                {
                    if (node.Method.DeclaringType == typeof(string))
                    {
                        MemberExpression expression3 = node.Object as MemberExpression;
                        this.builder.Append(expression3.Member.Name);
                        string str3 = "@P" + this.index++;
                        this.builder.AppendFormat(" LIKE ", new object[0]);
                        this.builder.Append(str3);
                        object obj4 = (node.Arguments[0] as ConstantExpression).Value;
                        this.cmd.Parameters.Add(str3, "%" + obj4 + "%", null, null, null, null, null);
                        return node;
                    }
                    if (node.Method.DeclaringType == typeof(Enumerable))
                    {
                        ConstantExpression expression4 = node.Arguments[0] as ConstantExpression;
                        MemberExpression expression5 = (MemberExpression) node.Arguments[1];
                        return this.VisitContains(node, expression4, expression5);
                    }
                    if (!typeof(IEnumerable).IsAssignableFrom(node.Method.DeclaringType))
                    {
                        throw new InvalidOperationException("Contains方法只能由数组、IEnumerable对象调用");
                    }
                    ConstantExpression source = node.Object as ConstantExpression;
                    MemberExpression member = (MemberExpression) node.Arguments[0];
                    return this.VisitContains(node, source, member);
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}

