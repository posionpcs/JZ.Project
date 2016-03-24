namespace FrameWork.Expressions
{
    using System;
    using System.Linq.Expressions;

    public class ExpressionModifier : ExpressionVisitor
    {
        private Expression newExpression;
        private Expression oldExpression;

        public ExpressionModifier(Expression newExpression, Expression oldExpression)
        {
            this.newExpression = newExpression;
            this.oldExpression = oldExpression;
        }

        public Expression Replace(Expression e)
        {
            return this.Visit(e);
        }

        public static Expression Replace(Expression e, Expression oldExpression, Expression newExpression)
        {
            return new ExpressionModifier(newExpression, oldExpression).Replace(e);
        }

        public override Expression Visit(Expression node)
        {
            if (node == this.oldExpression)
            {
                return base.Visit(this.newExpression);
            }
            return base.Visit(node);
        }
    }
}

