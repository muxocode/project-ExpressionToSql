using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;
using ExpressionToSQL.configuration;
using ExpressionToSQL.consultant;
using ExpressionToSql.consultant;

namespace ExpressionToSQL
{
    public class SqlParse: IQuery, ICommand
    {

        private class Configuration : IClassConfiguration
        {
            public bool FieldsInclude { get; set; }

            public bool PropsInclude { get; set; }
        }

        public SqlType SqlType;
        public IClassConfiguration ClassConfiguration { get; private set; }

        public SqlParse(IClassConfiguration configuration = null)
        {
            Init(configuration, SqlType.SqlServer);
        }
        public SqlParse(SqlType SqlType, IClassConfiguration configuration = null)
        {
            Init(configuration, SqlType.SqlServer);
        }

        private void Init(IClassConfiguration configuration, SqlType SqlType)
        {
            this.ClassConfiguration = configuration ?? new SqlConfiguration<object>();
            this.SqlType = SqlType;
        }

        public ISqlCommand<T> Command<T>(Expression<Func<T, bool>> expression = null)
        {
            var oConf = new SqlConfiguration<T>() { FieldsInclude = this.ClassConfiguration.FieldsInclude, PropsInclude = this.ClassConfiguration.PropsInclude };
            return new SqlGenerator<T>(this.SqlType,oConf, expression != null ? new Expression<Func<T, bool>>[] { expression } : null);
        }

        public ISqlQuery<T> Query<T>(Expression<Func<T, bool>> expression = null)
        {
            var oConf = new SqlConfiguration<T>() { FieldsInclude = this.ClassConfiguration.FieldsInclude, PropsInclude = this.ClassConfiguration.PropsInclude };

            return new SqlGenerator<T>(this.SqlType, oConf, expression != null ? new Expression<Func<T, bool>>[] { expression } : null);
        }

        public SqlParse Configure(bool? fieldsInclude = null, bool? propsInclude = null)
        {
            var oConf = new Configuration();

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
