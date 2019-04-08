using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;

namespace ExpressionToSQL
{
    public class SqlParser : IQuery, ICommand
    {
        SqlConfiguration Configuration { get; set; }

        IQueryConfiguration IQuery.Configuration => Configuration;

        ICommandConfiguration ICommand.Configuration => Configuration;

        public ISqlCommand<T> Command<T>(Expression<Func<T, bool>> Predicado)
        {
            throw new NotImplementedException();
        }

        public ICommand Configue(ICommandConfiguration CustomConfiguration)
        {
            throw new NotImplementedException();
        }

        public IQuery Configure(IQueryConfiguration Configuration)
        {
            throw new NotImplementedException();
        }

        public ISqlQuery<T> Query<T>(Expression<Func<T, bool>> Predicado)
        {
            throw new NotImplementedException();
        }

        ISqlCommand<T> ICommand.Command<T>(Expression<Func<T, bool>> Predicado)
        {
            throw new NotImplementedException();
        }

        ICommand ICommand.Configue(ICommandConfiguration CustomConfiguration)
        {
            throw new NotImplementedException();
        }

        IQuery IQuery.Configure(IQueryConfiguration Configuration)
        {
            throw new NotImplementedException();
        }

        ISqlQuery<T> IQuery.Query<T>(Expression<Func<T, bool>> Predicado)
        {
            throw new NotImplementedException();
        }
    }
}
