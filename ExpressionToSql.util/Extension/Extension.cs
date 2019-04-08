using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToSQL.util.Extension
{
    public static class Extension
    {
        public static String ToSql<T>(this Expression<Func<T, bool>> expression)
        {
            return ExpressionUtil.ToSql(expression);
        }
    }
}
