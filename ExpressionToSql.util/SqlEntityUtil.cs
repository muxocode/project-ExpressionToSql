using ExpressionToSQL.common.attributes;
using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExpressionToSQL.util
{
    public static class SqlEntityUtil
    {
        class SqlField
        {
            public string name;
            public string value;
        }

        public static IDictionary<string, string> GetKeysValues<T>(ICommandConfiguration Configuration, T Entidad)
        {
            if (Configuration == null)
            {
                throw new ArgumentNullException(nameof(Configuration));
            }

            var aSqlFields = new List<SqlField>();

            if (Configuration.PropsInclude.GetValueOrDefault())
            {
                var props = typeof(T).GetProperties()
                .Where(c => c.GetCustomAttributes(typeof(NonQuerable), false).Count() == 0)
                .Select(x => new SqlField() { name = x.Name, value = Entidad!=null ? SqlValueUtil.GetValue(x.GetValue(Entidad)) : null });

                aSqlFields.AddRange(props);
            }

            if (Configuration.FieldsInclude.GetValueOrDefault())
            {
                var fields = typeof(T).GetFields()
                .Where(c => c.GetCustomAttributes(typeof(NonQuerable), false).Count() == 0)
                .Select(x => new SqlField() { name = x.Name, value = Entidad != null ? SqlValueUtil.GetValue(x.GetValue(Entidad)) : null });

                aSqlFields.AddRange(fields);
            }


            if (!Configuration.IncludeId.GetValueOrDefault())
                aSqlFields = aSqlFields
                    .Where(x => x.name.ToUpper() != Configuration.PrimaryKeyTable.ToUpper())
                    .ToList();

            var Elementos = aSqlFields.ToDictionary(x => x.name, y => y.value);

            return Elementos;
        }

        public static IEnumerable<string> GetKeys<T>(ICommandConfiguration Configuration)
        {
            return GetKeysValues<T>(Configuration, default(T)).Keys.ToList();
        }

        public static IEnumerable<string> GetValues<T>(ICommandConfiguration Configuration, T Entidad)
        {
            return GetKeysValues<T>(Configuration, Entidad).Values.ToList();
        }

        public static string QueryTableNameSpace<T>(IQueryConfiguration Configuration)
        {
            string sResult = Configuration.TableName;

            if (Configuration.WihNoLock.GetValueOrDefault())
            {
                sResult = $"{sResult} WITH(NOLOCK)";
            }

            return sResult;
        }

        public static string CommandTableNameSpace<T>(ICommandConfiguration Configuration)
        {
            string sResult = Configuration.TableName;

            return sResult;
        }
    }
}
