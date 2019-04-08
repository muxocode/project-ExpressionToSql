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
        ICommandConfiguration Configuration { get; }
        ISqlCommand<T> Command<T>(Expression<Func<T, bool>> Predicado);
        ICommand Configue(ICommandConfiguration CustomConfiguration);
    }
}
