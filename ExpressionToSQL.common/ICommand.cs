using ExpressionToSQL.common.configuration;
using System;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ICommand
    {
        ISqlCommand<T> Command<T>(Expression<Func<T, bool>> expression= null);
        ICommand Configure(
            bool includeId,
            string primaryKeyTable = null,
            bool? fieldsInclude = null,
            bool? propsInclude = null,
            string tableName = null,
            string schema = null
            );

        ICommand Configure(
            string primaryKeyTable,
            bool? includeId = null,
            bool? fieldsInclude = null,
            bool? propsInclude = null,
            string tableName = null,
            string schema = null
            );

    }
}
