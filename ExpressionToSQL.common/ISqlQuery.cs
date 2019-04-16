using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ISqlQuery<T>: ISqlQueryOrdered<T>
    {

        string Count();

        string Max<TReturn>(Expression<Func<T, TReturn>> property);

        string Min<TReturn>(Expression<Func<T, TReturn>> property);

        ISqlQueryOrdered<T> OrderBy<TReturn>(params Expression<Func<T, TReturn>>[] property);
    }
}
