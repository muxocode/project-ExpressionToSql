using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;
using ExpressionToSQL.util.Extension;
using ExpressionToSQL.util;
using System.Linq;

namespace ExpressionToSQL
{
    internal class SqlGenerator<T> : ISqlQuery<T>, ISqlCommand<T>
    {
        public SqlConfiguration Configuration { get; set; }

        IQueryConfiguration ISqlQueryOrdered<T>.Configuration => Configuration;

        IQueryConfiguration ISqlCommand<T>.Configuration => Configuration;

        string Expression;
        string Table
        {
            get
            {
                var table = this.Configuration.TableName;
                if (this is ISqlQuery<T> && this.Configuration.WihNoLock.GetValueOrDefault())
                {
                    return $"{table} WITH(NOLOCK)";
                }

                return table;
            }
        }
        string Order;

        public SqlGenerator(SqlConfiguration configuration, Expression<Func<T, bool>> expression = null)
        {
            this.Configuration = configuration;
            this.Configuration.TableName = this.Configuration.TableName ?? typeof(T).Name;
            this.Expression = expression?.ToSql<T>();
        }


        public string Count()
        {
            return SqlStatementUtil.Select("COUNT(*)", this.Table, this.Expression);
        }

        public string Fisrt()
        {
            return SqlStatementUtil.Select("TOP(1) *", this.Table, this.Expression, this.Order);
        }

        public string Max<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlStatementUtil.Select($"MAX({ExpressionUtil.ToSqlSeleccion(property)})", this.Table, this.Expression);
        }

        public string Min<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlStatementUtil.Select($"MIN({ExpressionUtil.ToSqlSeleccion(property)})", this.Table, this.Expression);
        }

        public string Select()
        {
            return SqlStatementUtil.Select(
                string.Join(",", SqlEntityUtil.GetKeys<T>(this.Configuration)),
                this.Table,
                this.Expression,
                this.Order);
        }

        public string Select(int page, int registryNumber)
        {
            return SqlStatementUtil.Select(
                string.Join(",", SqlEntityUtil.GetKeys<T>(this.Configuration)),
                this.Table,
                this.Expression,
                this.Configuration.PrimaryKeyTable,
                page,
                registryNumber,
                this.Order);
        }



        public ISqlQueryOrdered<T> OrderBy<TReturn>(params Expression<Func<T, TReturn>>[] property)
        {
            if (property != null)
            {
                this.Order = ExpressionUtil.ToSqlSeleccion(property.First());

                foreach (var prop in property.Skip(1))
                {
                    this.Order += $",{ExpressionUtil.ToSqlSeleccion(prop)}";
                }
            }

            return this;
        }

        public string Insert(T entity)
        {
            var value_keys = SqlEntityUtil.GetKeysValues(this.Configuration, entity);

            return SqlStatementUtil.Insert(
                this.Table,
                string.Join(",", value_keys.Keys),
                string.Join(",", value_keys.Values),
                this.Configuration.PrimaryKeyTable);
        }

        public string Delete()
        {
            return SqlStatementUtil.Delete(this.Table, this.Expression);
        }

        public string Update(T entity)
        {
            var value_keys = SqlEntityUtil.GetKeysValues(this.Configuration, entity);

            return SqlStatementUtil.Update(
                this.Table,
                value_keys,
                this.Expression
                );
        }

        public string Update(IDictionary<string, string> valueFields)
        {
            return SqlStatementUtil.Update(
                this.Table,
                valueFields,
                this.Expression
                );
        }

        public string Insert(T[] entities)
        {
            var values = entities.Select(x => SqlEntityUtil.GetValues(this.Configuration, x));

            var keys = string.Join(",", SqlEntityUtil.GetKeys<T>(this.Configuration));

            return SqlStatementUtil.Insert(
                this.Table,
                keys,
                values,
                this.Configuration.PrimaryKeyTable);
        }
    }
}
