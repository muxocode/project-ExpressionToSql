using System;
using System.Collections.Generic;
using System.Text;
using ExpressionToSQL.common.configuration;

namespace ExpressionToSQL
{
    internal class SqlCommand<T> : common.ISqlCommand<T>
    {
        public IQueryConfiguration Configuration => throw new NotImplementedException();

        public string Delete()
        {
            throw new NotImplementedException();
        }

        public string Insert(params T[] Entidad)
        {
            throw new NotImplementedException();
        }

        public string Update(T Entidad)
        {
            throw new NotImplementedException();
        }

        public string Update(IDictionary<string, object> CamposValores)
        {
            throw new NotImplementedException();
        }
    }
}
