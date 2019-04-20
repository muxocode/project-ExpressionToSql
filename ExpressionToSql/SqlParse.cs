using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;
using ExpressionToSQL.configuration;

namespace ExpressionToSQL
{
    public class SqlParse : IQuery, ICommand
    {
        protected SqlClassConfiguration ClassConfiguration { get; private set; }

        IClassConfiguration IOperation<IQuery>.ClassConfiguration => ClassConfiguration;

        IClassConfiguration IOperation<ICommand>.ClassConfiguration => ClassConfiguration;

        public SqlParse(SqlClassConfiguration configuration = null)
        {
            this.ClassConfiguration = configuration ?? new SqlClassConfiguration();
        }

        public ISqlCommand<T> Command<T>(Expression<Func<T, bool>> expression = null)
        {
            return new SqlGenerator<T>(this.ClassConfiguration, expression);
        }

        public ISqlQuery<T> Query<T>(Expression<Func<T, bool>> expression = null)
        {
            return new SqlGenerator<T>(this.ClassConfiguration, expression);
        }

        public SqlParse Configure(bool? fieldsInclude = null, bool? propsInclude = null)
        {
            var oConf = this.ClassConfiguration.Clone();

            oConf.FieldsInclude = fieldsInclude.GetValueOrDefault(oConf.FieldsInclude);

            oConf.PropsInclude = propsInclude.GetValueOrDefault(oConf.PropsInclude);

            return new SqlParse(oConf);
        }

        IQuery IOperation<IQuery>.Configure(bool? FieldsInclude, bool? PropsInclude)
        {
            return Configure(FieldsInclude, PropsInclude);
        }

        ICommand IOperation<ICommand>.Configure(bool? FieldsInclude, bool? PropsInclude)
        {
            return Configure(FieldsInclude, PropsInclude);
        }
    }
}
