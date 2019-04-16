using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;

namespace ExpressionToSQL
{
    public class SqlParse : IQuery, ICommand
    {
        public SqlConfiguration Configuration { get; private set; }

        public SqlParse(SqlConfiguration configuration = null)
        {
            this.Configuration = configuration ?? new SqlConfiguration();
        }

        public ISqlCommand<T> Command<T>(Expression<Func<T, bool>> expression=null)
        {
            return new SqlGenerator<T>(this.Configuration, expression);
        }

        public ISqlQuery<T> Query<T>(Expression<Func<T, bool>> expression=null)
        {
            return new SqlGenerator<T>(this.Configuration, expression);
        }

        public IQuery Configure(bool? wihNoLock = null, bool? fieldsInclude = null, bool? propsInclude = null, string PrimaryKeyTable = null, string tableName = null)
        {
            var config = this.Configuration.Clone();

            config.WihNoLock = wihNoLock ?? config.WihNoLock;
            config.FieldsInclude = fieldsInclude ?? config.FieldsInclude;
            config.PropsInclude = propsInclude ?? config.PropsInclude;
            config.PrimaryKeyTable = PrimaryKeyTable ?? config.PrimaryKeyTable;
            config.TableName = tableName ?? config.TableName;

            return new SqlParse(config);
        }

        public ICommand Configure(bool? includeId=null)
        {
            var config = this.Configuration.Clone();

            config.IncludeId = includeId ?? config.IncludeId;

            return new SqlParse(config);
        }
    }
}
