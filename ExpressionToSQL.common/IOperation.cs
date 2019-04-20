using ExpressionToSQL.common.configuration;
using System;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface IOperation<T>
    {
        IClassConfiguration ClassConfiguration { get; }
        T Configure(bool? FieldsInclude=null,bool? PropsInclude=null);
    }
}
