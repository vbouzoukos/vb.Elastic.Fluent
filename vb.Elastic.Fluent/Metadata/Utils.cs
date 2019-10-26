using System;
using System.Linq.Expressions;
using System.Reflection;


namespace vb.Elastic.Fluent.Metadata
{
    internal class Utils
    {
        public static object GetObjectValue<T>(Expression<Func<T, object>> expression, T entity)
        {
            var memberExp = GetMemberInfo(expression);
            var propertyInfo = memberExp.Member as PropertyInfo;
            var item = propertyInfo.GetValue(entity, null);
            return item;
        }

        public static void SetObjectValue<T>(Expression<Func<T, object>> expression, T entity, object value)
        {
            var memberExp = GetMemberInfo(expression);
            var propertyInfo = memberExp.Member as PropertyInfo;
            propertyInfo.SetValue(entity, value);
        }

        public static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda != null)
            {
                MemberExpression memberExpr = null;

                if (lambda.Body.NodeType == ExpressionType.Convert)
                {
                    memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                }
                else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpr = lambda.Body as MemberExpression;
                }
                return memberExpr;
            }
            return null;
        }

        public static string ExpressionAttributeName<T>(Expression<Func<T, object>> entity) where T : class
        {
            var expression = (MemberExpression)entity.Body;
            string name = expression.Member.Name;
            return name;
        }
    }
}
