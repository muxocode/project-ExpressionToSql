using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ExpressionToSQL.common.configuration;

namespace ExpressionToSQL
{
    internal class SqlQuery<T> : common.ISqlQuery<T>
    {
        public IQueryConfiguration Configuration { get; set; }
        Expression<Func<T, bool>> Predicate;

        public SqlQuery(IQueryConfiguration Configuration, Expression<Func<T, bool>> Predicate=null)
        {
            this.Configuration = Configuration;
            this.Predicate = Predicate;
        }

        public string Count()
        {
            throw new NotImplementedException();
        }

        public string Fisrt()
        {
            throw new NotImplementedException();
        }

        public string Max<TPropiedad>(System.Linq.Expressions.Expression<Func<T, TPropiedad>> Propiedad)
        {
            throw new NotImplementedException();
        }

        public string Min<TPropiedad>(System.Linq.Expressions.Expression<Func<T, TPropiedad>> Propiedad)
        {
            throw new NotImplementedException();
        }

        public string Select()
        {
            throw new NotImplementedException();
        }

        public string Select(int Pagina, int NumeroRegistros)
        {
            throw new NotImplementedException();
        }
    }
}
