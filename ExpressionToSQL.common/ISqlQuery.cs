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
        ITableConfiguration Configuration { get; }
        ISqlQuery<T> Configure(
            string primaryKeyTable = null,
            string tableName = null,
            string schema = null
            );
        ISqlQueryOrdered<T> OrderBy<TReturn>(params Expression<Func<T, TReturn>>[] property);
        ISqlQueryGrouped<T> GroupBy<TReturn>(params Expression<Func<T, TReturn>>[] property);
        ISqlQuery<T> Where(Expression<Func<T, bool>> expression);
    }
}
