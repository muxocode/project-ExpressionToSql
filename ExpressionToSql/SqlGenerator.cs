using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;
using ExpressionToSQL.util.Extension;
using ExpressionToSQL.util;
using System.Linq;
using ExpressionToSQL.configuration;

namespace ExpressionToSQL
{
    internal class SqlGenerator<T> : ISqlQuery<T>, ISqlCommand<T>
    {
        protected SqlClassConfiguration ClassConfiguration { get; set; }
        public SqlCommandConfiguration<T> CommandConfiguration { get; set; }
        internal ISqlConsultant SqlConsultant { get; set; }
        

        string sExpression;
        List<Expression<Func<T, bool>>> eExpression;
        protected virtual string TableQuery
        {
            get
            {
                var table = this.TableCommand;
                /*
                if (this.Configuration.WihNoLock)
                {
                    table = $"{table} WITH(NOLOCK)";
                }*/

                return table;
            }
        }
        protected virtual string TableCommand
        {
            get
            {
                var table = $"{this.CommandConfiguration.TableName}";

                if (this.CommandConfiguration.Schema != null)
                {
                    table = $"{this.CommandConfiguration.Schema}.{table}";
                }

                return table;
            }
        }

        ITableConfiguration ISqlQuery<T>.Configuration => CommandConfiguration;

        ICommandConfiguration ISqlCommand<T>.Configuration => CommandConfiguration;

        protected string Group;

        protected string Order;

        string EmptyToNull(string text) => text != string.Empty ? text : null;

        public SqlGenerator(ISqlConsultant sqlConsultant, SqlClassConfiguration configuration, Expression<Func<T, bool>>[] expression = null, SqlCommandConfiguration<T> commandConfiguration = null)
        {
            this.SqlConsultant = sqlConsultant;
            this.ClassConfiguration = configuration;
            this.CommandConfiguration = commandConfiguration ?? new SqlCommandConfiguration<T>();
            this.sExpression = null;
            if (expression != null)
            {
                this.sExpression = EmptyToNull(String.Join(" AND ", expression?.Where(x => x != null).Select(x => x.ToSql<T>())));
            }
            this.eExpression = expression?.ToList();
        }


        public string Count()
        {
            return SqlConsultant.SelectGroup("COUNT(*)", this.TableQuery, this.sExpression, this.Group);
        }

        public string Fisrt()
        {
            return SqlConsultant.Select("TOP(1) *", this.TableQuery, this.sExpression, this.Order);
        }

        public string Max<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlConsultant.SelectGroup($"MAX({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.sExpression, this.Group);
        }

        public string Min<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlConsultant.SelectGroup($"MIN({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.sExpression, this.Group);
        }

        public string Avg<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlConsultant.SelectGroup($"AVG({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.sExpression, this.Group);
        }

        public string Sum<TReturn>(Expression<Func<T, TReturn>> property)
        {
            return SqlConsultant.SelectGroup($"SUM({ExpressionUtil.ToSqlSeleccion(property)})", this.TableQuery, this.sExpression, this.Group);
        }

        public string Select()
        {
            return SqlConsultant.Select(
                string.Join(",", SqlEntityUtil.GetKeys<T>(this.ClassConfiguration)),
                this.TableQuery,
                this.sExpression,
                this.Order);
        }

        public string Select(int page, int registryNumber)
        {
            return SqlConsultant.Select(
                string.Join(",", SqlEntityUtil.GetKeys<T>(this.ClassConfiguration, this.CommandConfiguration)),
                this.TableQuery,
                this.sExpression,
                this.CommandConfiguration.PrimaryKeyTable,
                page,
                registryNumber,
                this.Order);
        }



        public ISqlQueryOrdered<T> OrderBy<TReturn>(params Expression<Func<T, TReturn>>[] property)
        {

            string sOrder = String.Empty;

            if (property != null)
            {
                sOrder = ExpressionUtil.ToSqlSeleccion(property.First());

                foreach (var prop in property.Skip(1))
                {
                    sOrder += $",{ExpressionUtil.ToSqlSeleccion(prop)}";
                }
            }

            return new SqlGenerator<T>(this.SqlConsultant,ClassConfiguration, eExpression?.ToArray(), this.CommandConfiguration) { Order = EmptyToNull(sOrder) };
        }

        public string Insert(T entity)
        {
            var value_keys = SqlEntityUtil.GetKeysValues(this.ClassConfiguration, this.CommandConfiguration, entity);

            return SqlConsultant.Insert(
                this.TableCommand,
                string.Join(",", value_keys.Keys),
                string.Join(",", value_keys.Values),
                this.CommandConfiguration.PrimaryKeyTable);
        }

        public string Delete()
        {
            return SqlConsultant.Delete(this.TableCommand, this.sExpression);
        }

        public string Update(T entity)
        {
            var value_keys = SqlEntityUtil.GetKeysValues(this.ClassConfiguration, this.CommandConfiguration, entity);

            return SqlConsultant.Update(
                this.TableCommand,
                value_keys,
                this.sExpression
                );
        }

        public string Update(IDictionary<string, string> valueFields)
        {
            return SqlConsultant.Update(
                this.TableCommand,
                valueFields,
                this.sExpression
                );
        }

        public string Insert(T[] entities)
        {
            var values = entities.Select(x => SqlEntityUtil.GetValues(this.ClassConfiguration, this.CommandConfiguration, x));

            var keys = string.Join(",", SqlEntityUtil.GetKeys<T>(this.ClassConfiguration, this.CommandConfiguration));

            return SqlConsultant.Insert(
                this.TableCommand,
                keys,
                values,
                this.CommandConfiguration.PrimaryKeyTable);
        }

        public ISqlQueryGrouped<T> GroupBy<TReturn>(params Expression<Func<T, TReturn>>[] property)
        {

            string sGroup = String.Empty;

            if (property != null)
            {
                sGroup = ExpressionUtil.ToSqlSeleccion(property.First());

                foreach (var prop in property.Skip(1))
                {
                    sGroup += $",{ExpressionUtil.ToSqlSeleccion(prop)}";
                }
            }

            return new SqlGenerator<T>(this.SqlConsultant, ClassConfiguration, eExpression?.ToArray(), this.CommandConfiguration) { Group = EmptyToNull(sGroup) };
        }

        private SqlGenerator<T> Configure(bool? includeId, string primaryKeyTable, string tableName, string schema)
        {
            var oConf = this.CommandConfiguration.Clone();

            oConf.IncludeId = includeId.GetValueOrDefault(oConf.IncludeId);
            oConf.PrimaryKeyTable = primaryKeyTable ?? oConf.PrimaryKeyTable;
            oConf.TableName = tableName ?? oConf.TableName;
            oConf.Schema = schema ?? oConf.Schema;

            return new SqlGenerator<T>(this.SqlConsultant, ClassConfiguration, eExpression?.ToArray(), oConf);

        }

        ISqlQuery<T> ISqlQuery<T>.Configure(string primaryKeyTable, string tableName, string schema)
        {
            return Configure(null, primaryKeyTable, tableName, schema);
        }

        ISqlCommand<T> ISqlCommand<T>.Configure(bool? includeId, string primaryKeyTable, string tableName, string schema)
        {
            return Configure(includeId, primaryKeyTable, tableName, schema);
        }

        public ISqlQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            var aLista = new List<Expression<Func<T, bool>>>();
            aLista.AddRange((this.eExpression??new List<Expression<Func<T, bool>>>()).ToList());
            aLista.Add(expression);

            return new SqlGenerator<T>(this.SqlConsultant, this.ClassConfiguration, aLista.ToArray(), this.CommandConfiguration);
        }

        ISqlCommand<T> ISqlCommand<T>.Where(Expression<Func<T, bool>> expression)
        {
            var aLista = new List<Expression<Func<T, bool>>>();
            aLista.AddRange((this.eExpression ?? new List<Expression<Func<T, bool>>>()).ToList());
            aLista.Add(expression);

            return new SqlGenerator<T>(this.SqlConsultant, this.ClassConfiguration, aLista.ToArray(), this.CommandConfiguration);
        }
    }
}
