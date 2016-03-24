using FrameWork.Expressions;
using FrameWork.Utils;

namespace Framework.DAL.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class PartialEvaluator
    {
        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return (expression.NodeType != ExpressionType.Parameter);
        }

        private static bool CanBeEvaluatedLocally<T>(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter)
            {
                return false;
            }
            if (!(expression.Type != typeof(T)))
            {
                return (expression is MemberExpression);
            }
            return true;
        }

        public static Expression Eval(Expression expression)
        {
            return Eval(expression, new Func<Expression, bool>(PartialEvaluator.CanBeEvaluatedLocally));
        }

        public static Expression Eval<T>(Expression expression)
        {
            return Eval(expression, new Func<Expression, bool>(PartialEvaluator.CanBeEvaluatedLocally<T>));
        }

        public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        private class Nominator : ExpressionVisitor
        {
            private HashSet<Expression> candidates;
            private bool cannotBeEvaluated;
            private Func<Expression, bool> fnCanBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.candidates;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool cannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }
                    this.cannotBeEvaluated |= cannotBeEvaluated;
                }
                return expression;
            }
        }

        public class SubtreeEvaluator : ExpressionVisitor
        {
            private HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                Type type = e.Type;
                if (e.NodeType == ExpressionType.Convert)
                {
                    UnaryExpression expression = (UnaryExpression) e;
                    if (TypeHelper.GetNonNullableType(expression.Operand.Type) == TypeHelper.GetNonNullableType(type))
                    {
                        e = ((UnaryExpression) e).Operand;
                    }
                    else if (expression.Operand.Type.IsEnum && (expression.Operand.NodeType == ExpressionType.MemberAccess))
                    {
                        return Expression.Constant(Convert.ChangeType(MemberAccessor.Process(expression.Operand as MemberExpression), e.Type), e.Type);
                    }
                }
                ExpressionType nodeType = e.NodeType;
                if (nodeType != ExpressionType.Constant)
                {
                    if (nodeType != ExpressionType.MemberAccess)
                    {
                        throw new InvalidOperationException(string.Format("条件表达式中不支持 {0} 类型表达式", e.NodeType));
                    }
                    return Expression.Constant(MemberAccessor.Process(e as MemberExpression), e.Type);
                }
                if (e.Type == type)
                {
                    return e;
                }
                if (!(TypeHelper.GetNonNullableType(e.Type) == TypeHelper.GetNonNullableType(type)))
                {
                    throw new InvalidOperationException(string.Format("条件表达式未知表达式类型 {0}", e.NodeType));
                }
                return Expression.Constant(((ConstantExpression) e).Value, type);
            }

            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this.candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }
        }
    }
}

