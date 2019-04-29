using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;
using ExpressionToSQL.configuration;
using ExpressionToSQL.util;

namespace ExpressionToSQL
{
    public class SqlParse : IQuery, ICommand
    {
        protected SqlClassConfiguration ClassConfiguration { get; private set; }

        IClassConfiguration IOperation<IQuery>.ClassConfiguration => ClassConfiguration;

        IClassConfiguration IOperation<ICommand>.ClassConfiguration => ClassConfiguration;

        public ISqlConsultant SqlConsultant { get; set; } = new SqlServerConsultant();

        public SqlParse(SqlClassConfiguration configuration = null)
        {
            this.ClassConfiguration = configuration ?? new SqlClassConfiguration();
        }

        public ISqlCommand<T> Command<T>(Expression<Func<T, bool>> expression = null)
        {
            return new SqlGenerator<T>(this.SqlConsultant,this.ClassConfiguration, expression != null ? new Expression<Func<T, bool>>[] { expression } : null);
        }

        public ISqlQuery<T> Query<T>(Expression<Func<T, bool>> expression = null)
        {
            return new SqlGenerator<T>(this.SqlConsultant, this.ClassConfiguration, expression != null ? new Expression<Func<T, bool>>[] { expression } : null);
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
