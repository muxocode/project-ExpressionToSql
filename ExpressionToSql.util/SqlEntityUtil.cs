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

        public static IDictionary<string, string> GetKeysValues<T>(IClassConfiguration Configuration, ICommandConfiguration CommandConfiguration, T Entidad)
        {
            if (Configuration == null)
            {
                throw new ArgumentNullException(nameof(Configuration));
            }

            var aSqlFields = new List<SqlField>();

            if (Configuration.PropsInclude)
            {
                var props = typeof(T).GetProperties()
                .Where(c => c.GetCustomAttributes(typeof(NonQuerable), false).Count() == 0)
                .Select(x => new SqlField() { name = x.Name, value = Entidad!=null ? SqlValueUtil.GetValue(x.GetValue(Entidad)) : null });

                aSqlFields.AddRange(props);
            }

            if (Configuration.FieldsInclude)
            {
                var fields = typeof(T).GetFields()
                .Where(c => c.GetCustomAttributes(typeof(NonQuerable), false).Count() == 0)
                .Select(x => new SqlField() { name = x.Name, value = Entidad != null ? SqlValueUtil.GetValue(x.GetValue(Entidad)) : null });

                aSqlFields.AddRange(fields);
            }

            if (CommandConfiguration != null)
            {
                if (!CommandConfiguration.IncludeId)
                    aSqlFields = aSqlFields
                        .Where(x => x.name.ToUpper() != CommandConfiguration.PrimaryKeyTable.ToUpper())
                        .ToList();
            }
            var Elementos = aSqlFields.ToDictionary(x => x.name, y => y.value);

            return Elementos;
        }

        public static IEnumerable<string> GetKeys<T>(IClassConfiguration Configuration, ICommandConfiguration CommandConfiguration=null)
        {
            return GetKeysValues<T>(Configuration, CommandConfiguration, default(T)).Keys.ToList();
        }

        public static IEnumerable<string> GetValues<T>(IClassConfiguration Configuration, ICommandConfiguration CommandConfiguration, T Entidad)
        {
            return GetKeysValues<T>(Configuration, CommandConfiguration, Entidad).Values.ToList();
        }
    }
}
