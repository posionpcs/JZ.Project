namespace FrameWork.Expressions
{
    
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class MemberAccessor
    {
        private static ConcurrentDictionary<string, Func<object, object[], object>> cache = new ConcurrentDictionary<string, Func<object, object[], object>>();
        private static ConcurrentDictionary<string, Delegate> dcache = new ConcurrentDictionary<string, Delegate>();

        private static Func<object, object[], object> GetInstanceProperty(Expression e, MemberExpression topMember)
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            UnaryExpression expression3 = Expression.Convert(expression = Expression.Parameter(typeof(object), "local"), topMember.Member.DeclaringType);
            MemberExpression newExpression = topMember.Update(expression3);
            return Expression.Lambda<Func<object, object[], object>>(Expression.Convert(ExpressionModifier.Replace(e, topMember, newExpression), typeof(object)), new ParameterExpression[] { expression, expression2 = Expression.Parameter(typeof(object[]), "args") }).Compile();
        }

        public static ConstantExpression GetRootConstant(MemberExpression e)
        {
            if (e.Expression == null)
            {
                return null;
            }
            ExpressionType nodeType = e.Expression.NodeType;
            if (nodeType != ExpressionType.Constant)
            {
                if (nodeType == ExpressionType.MemberAccess)
                {
                    return GetRootConstant(e.Expression as MemberExpression);
                }
                return null;
            }
            return (e.Expression as ConstantExpression);
        }

        public static MemberExpression GetRootMember(MemberExpression e)
        {
            if ((e.Expression == null) || (e.Expression.NodeType == ExpressionType.Constant))
            {
                return e;
            }
            if (e.Expression.NodeType == ExpressionType.MemberAccess)
            {
                return GetRootMember(e.Expression as MemberExpression);
            }
            return null;
        }

        public static Func<object, object[], object> GetStaticProperty(Expression e, MemberExpression topMember)
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            return Expression.Lambda<Func<object, object[], object>>(Expression.Convert(e, typeof(object)), new ParameterExpression[] { expression = Expression.Parameter(typeof(object), "local"), expression2 = Expression.Parameter(typeof(object[]), "args") }).Compile();
        }

        public static object Process(MemberExpression e)
        {
            Func<string, Func<object, object[], object>> valueFactory = null;
            Func<string, Func<object, object[], object>> func4 = null;
            MemberExpression topMember = GetRootMember(e);
            if (topMember == null)
            {
                throw new InvalidOperationException("需计算的条件表达式只支持由 MemberExpression 和 ConstantExpression 组成的表达式");
            }
            if (topMember.Expression == null)
            {
                if (valueFactory == null)
                {
                    valueFactory = key => GetStaticProperty(e, topMember);
                }
                return cache.GetOrAdd(e.ToString(), valueFactory)(null, null);
            }
            if (func4 == null)
            {
                func4 = key => GetInstanceProperty(e, topMember);
            }
            return cache.GetOrAdd(e.ToString(), func4)((topMember.Expression as ConstantExpression).Value, null);
        }

        public static object Process<TModel>(TModel instance, MemberInfo m)
        {
            return ModelCompiler<TModel>.Compile(m)(instance);
        }

        public static TProperty Process<TModel, TProperty>(Expression<Func<TModel, TProperty>> e, TModel instance)
        {
            return Compiler<TModel, TProperty>.Compile(e)(instance);
        }

        private static class Compiler<TModel, TProperty>
        {
            private static readonly ConcurrentDictionary<MemberInfo, Func<TModel, TProperty>> cache;

            static Compiler()
            {
                MemberAccessor.Compiler<TModel, TProperty>.cache = new ConcurrentDictionary<MemberInfo, Func<TModel, TProperty>>();
            }

            public static Func<TModel, TProperty> Compile(Expression<Func<TModel, TProperty>> e)
            {
                MemberExpression body = e.Body as MemberExpression;
                body.ThrowIfNull<MemberExpression>("e.Body is not a MemberExpression");
                return MemberAccessor.Compiler<TModel, TProperty>.cache.GetOrAdd(body.Member, key => e.Compile());
            }
        }

        private static class ModelCompiler<TModel>
        {
            private static readonly ConcurrentDictionary<MemberInfo, Func<TModel, object>> cache;

            static ModelCompiler()
            {
                MemberAccessor.ModelCompiler<TModel>.cache = new ConcurrentDictionary<MemberInfo, Func<TModel, object>>();
            }

            public static Func<TModel, object> Compile(MemberInfo m)
            {
                return MemberAccessor.ModelCompiler<TModel>.cache.GetOrAdd(m, delegate (MemberInfo key) {
                    ParameterExpression expression;
                    return Expression.Lambda<Func<TModel, object>>(Expression.Convert(Expression.MakeMemberAccess(expression = Expression.Parameter(typeof(TModel), "model"), m), typeof(object)), new ParameterExpression[] { expression }).Compile();
                });
            }
        }
    }
}

