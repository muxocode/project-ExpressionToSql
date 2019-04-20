using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ISqlCommand<T>
    {
        ICommandConfiguration Configuration { get; }

        ISqlCommand<T> Configure(
            bool? includeId=null,
            string primaryKeyTable = null,
            string tableName = null,
            string schema = null
            );

        string Insert(T entity);

        string Insert(T[] entities);

        string Delete();

        string Update(T entity);

        string Update(IDictionary<string, string> valueFields);
    }
}
