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
using ExpressionToSql.consultant;

namespace ExpressionToSQL
{
    internal class SqlGenerator<T> : ISqlQuery<T>, ISqlCommand<T>
    {
        protected SqlConfiguration<T> Configuration { get; set; }       

        string sExpression;
        List<Expression<Func<T, bool>>> eExpression;

        IQueryConfiguration ISqlQuery<T>.Configuration => Configuration;

        ICommandConfiguration ISqlCommand<T>.Configuration => Configuration;

        protected string Group;

        protected string Order;

        string EmptyToNull(string text) => text != string.Empty ? text : null;
        SqlType Type;
        public SqlGenerator(SqlType type, SqlConfiguration<T> configuration, Expression<Func<T, bool>>[] expression = null)
        {
            this.Configuration =  configuration;
            this.sExpression = null;
            if (expression != null)
            {
                this.sExpression = EmptyToNull(String.Join(" AND ", expression?.Where(x => x != null).Select(x => x.ToSql<T>())));
            }
            this.eExpression = expression?.ToList();
            Type = type;
        }


        public string Count()
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Count(this.sExpression, this.Group);
        }

        public string Fisrt()
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Fisrt(this.sExpression, this.Order);
        }

        public string Max<TReturn>(Expression<Func<T, TReturn>> property)
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Max(ExpressionUtil.ToSqlSeleccion(property), this.sExpression, this.Group);
        }

        public string Min<TReturn>(Expression<Func<T, TReturn>> property)
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Min(ExpressionUtil.ToSqlSeleccion(property), this.sExpression, this.Group);

        }

        public string Avg<TReturn>(Expression<Func<T, TReturn>> property)
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Avg(ExpressionUtil.ToSqlSeleccion(property), this.sExpression, this.Group);
        }

        public string Sum<TReturn>(Expression<Func<T, TReturn>> property)
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Sum(ExpressionUtil.ToSqlSeleccion(property), this.sExpression, this.Group);
        }

        public string Select()
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Select(
                SqlEntityUtil.GetKeys<T>(this.Configuration),
                this.sExpression,
                this.Order);
        }

        public string Select(int page, int registryNumber)
        {
            var SqlConsultant = SqlConsultantHelper.CreateQuerier(Type, this.Configuration);
            return SqlConsultant.Select(
                SqlEntityUtil.GetKeys<T>(this.Configuration),
                this.sExpression,
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

            return new SqlGenerator<T>(this.Type, this.Configuration, eExpression?.ToArray()) { Order = EmptyToNull(sOrder) };
        }

        public string Insert(T entity)
        {
            var SqlConsultant = SqlConsultantHelper.CreateCommander(Type, this.Configuration);
            var value_keys = SqlEntityUtil.GetKeysValues(this.Configuration, entity);

            return SqlConsultant.Insert(
                value_keys.Keys,
                value_keys.Values);
        }

        public string Delete()
        {
            var SqlConsultant = SqlConsultantHelper.CreateCommander(Type, this.Configuration);
            return SqlConsultant.Delete(this.sExpression);
        }

        public string Update(T entity)
        {
            var SqlConsultant = SqlConsultantHelper.CreateCommander(Type, this.Configuration);
            var value_keys = SqlEntityUtil.GetKeysValues(this.Configuration, entity);

            return SqlConsultant.Update(
                value_keys,
                this.sExpression
                );
        }

        public string Update(IDictionary<string, string> valueFields)
        {
            var SqlConsultant = SqlConsultantHelper.CreateCommander(Type, this.Configuration);
            return SqlConsultant.Update(
                valueFields,
                this.sExpression
                );
        }

        public string Insert(T[] entities)
        {
            var SqlConsultant = SqlConsultantHelper.CreateCommander(Type, this.Configuration);
            var values = entities.Select(x => SqlEntityUtil.GetValues(this.Configuration, x));

            var keys = SqlEntityUtil.GetKeys<T>(this.Configuration, this.Configuration);

            return SqlConsultant.Insert(
                keys,
                values);
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

            return new SqlGenerator<T>(this.Type, this.Configuration, eExpression?.ToArray()) { Group = EmptyToNull(sGroup) };
        }

        private SqlGenerator<T> Configure(bool? includeId, string primaryKeyTable, string tableName, string schema)
        {
            var oConf = this.Configuration.Clone();

            oConf.IncludeId = includeId.GetValueOrDefault(oConf.IncludeId);
            oConf.PrimaryKeyTable = primaryKeyTable ?? oConf.PrimaryKeyTable;
            oConf.TableName = tableName ?? oConf.TableName;
            oConf.Schema = schema ?? oConf.Schema;

            return new SqlGenerator<T>(this.Type, oConf, eExpression?.ToArray());

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

            return new SqlGenerator<T>(this.Type, this.Configuration, aLista.ToArray());
        }

        ISqlCommand<T> ISqlCommand<T>.Where(Expression<Func<T, bool>> expression)
        {
            var aLista = new List<Expression<Func<T, bool>>>();
            aLista.AddRange((this.eExpression ?? new List<Expression<Func<T, bool>>>()).ToList());
            aLista.Add(expression);

            return new SqlGenerator<T>(this.Type, this.Configuration, aLista.ToArray());
        }
    }
}
