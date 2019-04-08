using ExpressionToSQL.common.error;
using ExpressionToSQL.util.enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToSQL.util
{
    public static class ExpressionUtil
    {

        public static String ToSql<T>(Expression<Func<T, bool>> expression)
        {
            try
            {

                String sResult = null;

                if (expression != null)
                {

                    var body = expression.Body;

                    if ((body as BinaryExpression) != null)
                    {
                        sResult = GetExpresion<BinaryExpression>(body);
                    }
                    else if ((body as MethodCallExpression) != null)
                    {
                        sResult = GetExpresion<MethodCallExpression>(expression.Body);
                    }
                    else if ((body as MemberExpression) != null)
                    {
                        sResult = GetExpresion<MemberExpression>(expression.Body);
                    }
                    else if ((body as UnaryExpression) != null)
                    {
                        sResult = GetExpresion<UnaryExpression>(expression.Body);
                    }

                }

                return sResult;
            }
            catch (Exception oEx)
            {
                throw new ExpresionToSqlException("Error at calculating SQL", oEx);
            }
        }

        private static string GetExpresion<T>(Expression Body) where T : Expression
        {
            var builder = String.Empty;

            IDictionary<string, Object> expando = new Dictionary<string, Object>();

            switch (typeof(T).Name)
            {
                case "BinaryExpression":
                    builder = ToSql((BinaryExpression)Body);

                    break;
                case "MethodCallExpression":
                    builder = ToSql((MethodCallExpression)Body);

                    break;

                case "MemberExpression":
                    builder = ToSql((MemberExpression)Body, TipoOperacion.Default);

                    break;

                case "UnaryExpression":
                    builder = ToSql((UnaryExpression)Body, TipoOperacion.Default);

                    break;
            }

            return builder;
        }

        private static string ToSql(UnaryExpression Body, TipoOperacion linkingType)
        {
            string sResult = null;
            if (Body.Operand as MethodCallExpression != null)
            {
                sResult = ToSql(Body.Operand as MethodCallExpression);
                sResult = sResult
                    .Replace(" IN (", " NOT IN (")
                    .Replace(" LIKE N", " NOT LIKE N");
            }
            else if ((Body.Operand as BinaryExpression) != null)
            {
                sResult = $"NOT ({ToSql(Body.Operand as BinaryExpression)})";
            }
            else if ((Body.Operand as MemberExpression) != null)
            {
                sResult = $"NOT ({ToSql(Body.Operand as MemberExpression, linkingType)})";
            }
            else if ((Body.Operand as UnaryExpression) != null)
            {
                sResult = $"NOT ({ToSql(Body.Operand as UnaryExpression, linkingType)})";
            }
            else
            {
                throw new ExpresionToSqlException("No se admite el tipo de negación");
            }

            return sResult;
        }

        private static string ToSql(MemberExpression Body, TipoOperacion linkingType)
        {
            string propertyValueResult;
            string propertyName;
            string link;

            propertyValueResult = SqlValueUtil.GetValue(true);

            propertyName = Body.Member.Name;

            link = GetOperator(linkingType, propertyValueResult);

            return string.Format("{0} {1} {2}", propertyName, "=", propertyValueResult);
        }

        private static string ToSql(MethodCallExpression MethodBody)
        {
            string propertyValueResult;
            string propertyName;
            string sResult = null;

            switch (MethodBody.Method.Name)
            {
                case "StartsWith":
                    propertyValueResult = MethodBody.Arguments.First().ToString();
                    propertyValueResult = propertyValueResult.Replace("\"", "");
                    propertyValueResult = $"{propertyValueResult}%";
                    propertyValueResult = SqlValueUtil.GetValue(propertyValueResult);

                    propertyName = MethodBody.Object.ToString().Split('.').Last();

                    sResult = string.Format("{0} {1} {2}", propertyName, "LIKE", propertyValueResult);

                    break;
                case "EndsWith":
                    propertyValueResult = MethodBody.Arguments.First().ToString();
                    propertyValueResult = propertyValueResult.Replace("\"", "");
                    propertyValueResult = $"%{propertyValueResult}";
                    propertyValueResult = SqlValueUtil.GetValue(propertyValueResult);

                    propertyName = MethodBody.Object.ToString().Split('.').Last();

                    sResult = string.Format("{0} {1} {2}", propertyName, "LIKE", propertyValueResult);

                    break;

                case "Contains":
                    var Nombres = new List<string>() { "List", "Enumerable", "Array" };

                    if (Nombres.Any(MethodBody.Method.DeclaringType.Name.StartsWith))
                    {
                        IEnumerable Lista = null;

                        if (MethodBody.Method.DeclaringType.Name.StartsWith("List", StringComparison.Ordinal))
                        {
                            Lista = GetValueExpression((Expression)MethodBody.Object) as IEnumerable;
                            propertyName = MethodBody.Arguments.First().ToString().Split('.').Last();
                        }
                        else
                        {
                            Lista = GetValueExpression(MethodBody.Arguments.First() as Expression) as IEnumerable;
                            propertyName = MethodBody.Arguments.Last().ToString().Split('.').Last();
                        }


                        string sValueList = string.Empty;
                        foreach (var oL in Lista)
                        {
                            string sComa = (sValueList == string.Empty) ? "" : ",";
                            sValueList += $"{sComa}{SqlValueUtil.GetValue(oL)}";
                        }
                        if (sValueList == string.Empty)
                        {
                            return string.Format("{0} {1} {2}", "1", "=", "0");
                        }
                        else
                        {
                            sResult = string.Format("{0} {1} ({2})", propertyName, "IN", sValueList);

                        }

                    }
                    else if (MethodBody.Method.DeclaringType.Name == "String")
                    {
                        propertyValueResult = MethodBody.Arguments.First().ToString();
                        propertyValueResult = propertyValueResult.Replace("\"", "");
                        propertyValueResult = $"%{propertyValueResult}%";
                        propertyValueResult = SqlValueUtil.GetValue(propertyValueResult);

                        propertyName = MethodBody.Object.ToString().Split('.').Last();

                        sResult = string.Format("{0} {1} {2}", propertyName, "LIKE", propertyValueResult);
                    }
                    else
                    {
                        throw new ExpresionToSqlException("No existe implemenetación para ese tipo de objeto");
                    }
                    break;
            }

            if (sResult == null)
            {
                throw new ExpresionToSqlException("No existe converisón SQL para ese método");
            }

            return sResult;
        }

        private static string ToSql(BinaryExpression body)
        {

            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {

                string propertyName = GetPropertyName(body);
                Expression propertyValue = body.Right;
                string propertyValueResult = SqlValueUtil.GetValue(GetValueExpression(propertyValue));
                string opr = GetOperator((TipoOperacion)body.NodeType, propertyValueResult);



                return string.Format("{0} {1} {2}", propertyName, opr, propertyValueResult);
            }
            else
            {
                string link = GetOperator((TipoOperacion)body.NodeType, null);

                var Left = body.Left;
                var Right = body.Right;

                return string.Format("({0}) {1} ({2})", AnalizePart(Left, body), link, AnalizePart(Right, body));
            }
        }

        private static string AnalizePart(Expression Expr, BinaryExpression body)
        {
            String sResult = null;

            if (Expr as BinaryExpression != null)
            {
                sResult = ToSql(Expr as BinaryExpression);
            }
            else if (Expr as MethodCallExpression != null)
            {
                var MethodBody = Expr as MethodCallExpression;
                sResult = ToSql(MethodBody);
            }
            else if (Expr as MemberExpression != null)
            {
                var MethodBody = Expr as MemberExpression;
                sResult = ToSql(MethodBody, (TipoOperacion)body.NodeType);
            }
            else if (Expr as UnaryExpression != null)
            {
                var MethodBody = Expr as UnaryExpression;
                sResult = ToSql(MethodBody, (TipoOperacion)body.NodeType);
            }

            return sResult;
        }


        private static object GetValueExpression(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));

            var getterLambda = Expression.Lambda<Func<object>>(objectMember);

            var getter = getterLambda.Compile();

            return getter();
        }

        private static string GetPropertyName(BinaryExpression body)
        {
            string propertyName = body.Left.ToString().Split(new char[] { '.' })[1];

            if (body.Left.NodeType == ExpressionType.Convert)
            {
                propertyName = propertyName.Replace(")", string.Empty);
            }

            return propertyName;
        }

        private static string GetOperator(TipoOperacion type, string value)
        {
            switch (type)
            {
                case TipoOperacion.Equal:
                    return value != "null" ? "=" : "IS";
                case TipoOperacion.NotEqual:
                    return value != "null" ? "<>" : "IS NOT";
                case TipoOperacion.LessThan:
                    return "<";
                case TipoOperacion.GreaterThan:
                    return ">";
                case TipoOperacion.LessThanOrEqual:
                    return "<=";
                case TipoOperacion.GreaterThanOrEqual:
                    return ">=";
                case TipoOperacion.AndAlso:
                case TipoOperacion.And:
                    return "AND";
                case TipoOperacion.Or:
                case TipoOperacion.OrElse:
                    return "OR";
                case TipoOperacion.Default:
                    return string.Empty;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
