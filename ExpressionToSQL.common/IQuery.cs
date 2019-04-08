using ExpressionToSQL.common.configuration;
using System;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface IQuery
    {
        IQueryConfiguration Configuration { get; }
        ISqlQuery<T> Query<T>(Expression<Func<T, bool>> Predicado);
        IQuery Configure(IQueryConfiguration Configuration);
    }
}
