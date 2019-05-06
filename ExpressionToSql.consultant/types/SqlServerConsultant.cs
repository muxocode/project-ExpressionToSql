using ExpressionToSQL.common;
using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionToSQL.consultant.types
{
    internal class SqlServerConsultant : ISqlQueryConsultant, ISqlCommandConsultant
    {
        public IQueryConfiguration QueryConfiguration { get; set; }
        public ICommandConfiguration CommandConfiguration { get; set; }

        protected virtual string TableQuery
        {
            get
            {
                var table = $"{this.QueryConfiguration.TableName}";

                if (this.QueryConfiguration.Schema != null)
                {
                    table = $"{this.QueryConfiguration.Schema}.{table}";
                }

                if (this.QueryConfiguration.WithNoLock)
                {
                    table = $"{table} WITH(NOLOCK)";
                }

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

        public string Select(IEnumerable<string> keys, string expression, string order = null)
        {
            string sResult = String.Format($"SELECT {String.Join(",", keys)} FROM {TableQuery}");

            if (expression != null)
                sResult = $"{sResult} WHERE {expression}";
            if (order != null)
                sResult = $"{sResult} ORDER BY {order}";

            return sResult;
        }

        public string SelectGroup(IEnumerable<string> keys, string expression, string groupby = null)
        {
            string sResult = Select(keys, expression);
            if (groupby != null)
                sResult = $"{sResult} GROUP BY {groupby}";

            return sResult;
        }

        public string Fisrt(string expression, string orderby = null)
        {
            return Select(new List<string>(){ "TOP(1) *"}, expression, orderby);
        }

        public string Count(string expression, string groupby = null)
        {
            return SelectGroup(new List<string>() { "COUNT(*)" }, expression, groupby);
        }

        public string Max(string key, string expression, string groupby = null)
        {
            return SelectGroup(new List<string>() { $"MAX({key})" }, expression, groupby);
        }

        public string Min(string key, string expression, string groupby = null)
        {
            return SelectGroup(new List<string>() { $"MIN({key})" }, expression, groupby);
        }

        public string Avg(string key, string expression, string groupby = null)
        {
            return SelectGroup(new List<string>() { $"AVG({key})" }, expression, groupby);
        }

        public string Sum(string key, string expression, string groupby = null)
        {
            return SelectGroup(new List<string>() { $"SUM({key})" }, expression, groupby);
        }

        public string Select(IEnumerable<string> keys, string expression, int page, int registers, string order = null)
        {
            return String.Format(@"WITH C AS
            ( 
              SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS rownum,
                {1}
              FROM {2}
              {5}
            )
            SELECT {1}
            FROM C
            WHERE rownum BETWEEN ({3}-1) * {4} + 1 AND {3} * {4} ORDER BY {6}",
                this.QueryConfiguration.PrimaryKeyTable,
                string.Join(",", keys),
                TableQuery,
                page,
                registers,
                expression == null ? String.Empty : $"WHERE {expression}",
                order ?? this.QueryConfiguration.PrimaryKeyTable
                );
        }

        public string Insert(IEnumerable<string> keys, IEnumerable<string> values)
        {
            return string.Format("INSERT INTO {0} ({1}) OUTPUT inserted.{3} VALUES ({2})",
                                     TableCommand,
                                     String.Join(",", keys),
                                     String.Join(",", values),
                                     this.CommandConfiguration.PrimaryKeyTable);
        }

        public string Insert(IEnumerable<string> keys, IEnumerable<IEnumerable<string>> values)
        {
            string sResult = String.Empty; ;
            string sAux = String.Empty;


            foreach (var objectValues in values.Reverse())
            {
                sResult = $"{string.Format("SELECT {0} {1}", string.Join(",", objectValues), sAux)} {sResult}";
                sAux = "UNION ALL";
            }


            sResult = string.Format("INSERT INTO {0} ({1}) OUTPUT Inserted.{3} {2}", TableCommand, string.Join(",", keys), sResult, this.CommandConfiguration.PrimaryKeyTable);

            return sResult;
        }

        public string Delete(string expression = null)
        {
            return string.Format("DELETE FROM {0}{1}", TableCommand, expression != null ? $" WHERE {expression}" : String.Empty);
        }

        public string Update(IDictionary<string, string> FieldNameValues, string expression = null)
        {
            string sResult;
            var Elementos = (from oObj in FieldNameValues
                             select String.Format("{0} = {1}", oObj.Key, oObj.Value)
                            ).ToArray();

            sResult = string.Format("UPDATE {0} SET {1}", TableCommand, string.Join(",", Elementos));

            if (expression != null)
            {
                sResult = $"{sResult} WHERE {expression}";
            }


            return sResult;
        }
    }
}
