using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ISqlQuery<T>: ISqlQueryOrdered<T>, ISqlQueryGrouped<T>
    {
        ISqlQueryOrdered<T> OrderBy<TReturn>(params Expression<Func<T, TReturn>>[] property);
        ISqlQueryGrouped<T> GroupBy<TReturn>(params Expression<Func<T, TReturn>>[] property);
    }
}
