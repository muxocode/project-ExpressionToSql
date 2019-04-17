using ExpressionToSQL.common.configuration;
using System;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ISqlQueryGrouped<T>
    {
        IQueryConfiguration Configuration { get; }
        string Count();

        string Max<TReturn>(Expression<Func<T, TReturn>> property);

        string Min<TReturn>(Expression<Func<T, TReturn>> property);
        string Sum<TReturn>(Expression<Func<T, TReturn>> property);
        string Avg<TReturn>(Expression<Func<T, TReturn>> property);

    }
}
