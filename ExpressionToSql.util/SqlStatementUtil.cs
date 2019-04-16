using ExpressionToSQL.common.error;
using ExpressionToSQL.util.enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionToSQL.util
{
    public static class SqlStatementUtil
    {
        public static string Select(string select, string table, string expression, string order = null)
        {
            string sResult = String.Format($"SELECT {select} FROM {table}");
            if (expression != null)
                sResult = $"{sResult} WHERE {expression}";
            if (order != null)
                sResult = $"{sResult} ORDER BY {order}";

            return sResult;
        }

        public static string Select(string select, string table, string expression, string row_number_name, int page, int registers, string order = null)
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
                row_number_name,
                select,
                table,
                page,
                registers,
                expression == null ? String.Empty : $"WHERE {expression}",
                order ?? row_number_name
                );
        }

        public static string Insert(string table, string keys, string values, string keyTable)
        {
            return string.Format("INSERT INTO {0} ({1}) OUTPUT inserted.{3} VALUES ({2})",
                                     table,
                                     keys,
                                     values,
                                     keyTable);
        }

        public static string Insert(string table, string keys, IEnumerable<IEnumerable<string>> values, string keyTable)
        {
            string sResult = null;

            foreach(var objectValues in values)
            {
                var sAux = sResult!=null ? " UNION ALL " : "";
                sResult += string.Format("SELECT {0} {1}", string.Join(",", objectValues), sAux);
            }

            sResult = string.Format("INSERT INTO {0} ({1}) OUTPUT Inserted.{3} {2}", table, string.Join(",", keys), sResult, keyTable);

            return sResult;
        }

        public static string Delete(string table, string expression = null)
        {
            return string.Format("DELETE FROM {0}{1}", table, expression != null ? $" WHERE {expression}" : String.Empty);
        }

        public static string Update(string table, IDictionary<string, string> FieldNameValues, string expression = null)
        {
            string sResult;
            var Elementos = (from oObj in FieldNameValues
                             select String.Format("{0} = {1}", oObj.Key, oObj.Value)
                            ).ToArray();

            if (expression != null)
            {
                sResult= string.Format("UPDATE {0} SET {1} WHERE {2}", table, string.Join(",", Elementos), expression);
            }
            else
            {
                sResult= string.Format("UPDATE {0} SET {1}", table, string.Join(",", Elementos));
            }

            return sResult;
        }
    }
}
