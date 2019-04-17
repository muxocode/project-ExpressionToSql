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
        ISqlQuery<T> Query<T>(Expression<Func<T, bool>> expression=null);
        IQuery Configure(
            bool wihNoLock,
            bool? fieldsInclude = null,
            bool? propsInclude = null,
            string tableName = null,
            string schema = null);
    }
}
