using ExpressionToSQL.common.configuration;
using System;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ISqlQueryOrdered<T>
    {
        string Fisrt();

        string Select();

        string Select(int page, int registryNumber);
    }
}
