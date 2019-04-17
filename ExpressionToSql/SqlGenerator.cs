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
        string TableQuery
        {
            get
            {
                var table = this.Configuration.TableName;
                if (this.Configuration.WihNoLock)
                {
                    table = $"{table} WITH(NOLOCK)";
                }

                if (this.Configuration.Schema != null)
                {
                    table = $"{this.Configuration.Schema}.{table}";
                }

                return table;
            }
        }
        string TableCommand
        {
            get
            {
                var table = this.Configuration.TableName;

                if (this.Configuration.Schema != null)
                {
                    table = $"{this.Configuration.Schema}.{table}";
                }

                return table;
            }
        }

        IQueryConfiguration ISqlQueryGrouped<T>.Configuration => throw new NotImplementedException();

        string Group;

        string Order;

        public SqlGenerator(SqlConfiguration configuration, Expression<Func<T, bool>> expression = null)
        {
            this.Configuration = configuration;
            this.Configuration.TableName = this.Configuration.TableName ?? typeof(T).Name;
            this.Expression = expression?.ToSql<T>();
        }


        public string Count()
        {
            return SqlStatementUtil.SelectGroup("COUNT(*)", this.TableQuery, this.Expression, this.Group);
        }

        public string Fisrt()
        {
            return SqlStatementUtil.Select("TOP(1) *", this.TableQuery, this.Expression, this.Order);
        }

        public string Max<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlStatementUtil.SelectGroup($"MAX({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.Expression, this.Group);
        }

        public string Min<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlStatementUtil.SelectGroup($"MIN({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.Expression, this.Group);
        }

        public string Select()
        {
            return SqlStatementUtil.Select(
                string.Join(",", SqlEntityUtil.GetKeys<T>(this.Configuration)),
                this.TableQuery,
                this.Expression,
                this.Order);
        }

        public string Select(int page, int registryNumber)
        {
            return SqlStatementUtil.Select(
                string.Join(",", SqlEntityUtil.GetKeys<T>(this.Configuration)),
                this.TableQuery,
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
                this.TableCommand,
                string.Join(",", value_keys.Keys),
                string.Join(",", value_keys.Values),
                this.Configuration.PrimaryKeyTable);
        }

        public string Delete()
        {
            return SqlStatementUtil.Delete(this.TableCommand, this.Expression);
        }

        public string Update(T entity)
        {
            var value_keys = SqlEntityUtil.GetKeysValues(this.Configuration, entity);

            return SqlStatementUtil.Update(
                this.TableCommand,
                value_keys,
                this.Expression
                );
        }

        public string Update(IDictionary<string, string> valueFields)
        {
            return SqlStatementUtil.Update(
                this.TableCommand,
                valueFields,
                this.Expression
                );
        }

        public string Insert(T[] entities)
        {
            var values = entities.Select(x => SqlEntityUtil.GetValues(this.Configuration, x));

            var keys = string.Join(",", SqlEntityUtil.GetKeys<T>(this.Configuration));

            return SqlStatementUtil.Insert(
                this.TableCommand,
                keys,
                values,
                this.Configuration.PrimaryKeyTable);
        }

        public ISqlQueryGrouped<T> GroupBy<TReturn>(params Expression<Func<T, TReturn>>[] property)
        {
            if (property != null)
            {
                this.Group = ExpressionUtil.ToSqlSeleccion(property.First());

                foreach (var prop in property.Skip(1))
                {
                    this.Group += $",{ExpressionUtil.ToSqlSeleccion(prop)}";
                }
            }

            return this;
        }

        public string Sum<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlStatementUtil.SelectGroup($"SUM({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.Expression, this.Group);
        }

        public string Avg<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlStatementUtil.SelectGroup($"AVG({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.Expression, this.Group);
        }
    }
}
