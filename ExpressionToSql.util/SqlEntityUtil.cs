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
        public static IDictionary<string, string> GetKeysValues<T, C>(C configuration, T entity) where C : IClassConfiguration, ICommandConfiguration => GetKeysValues<T>(configuration, configuration, entity);
        public static IDictionary<string, string> GetKeysValues<T>(IClassConfiguration configuration, ICommandConfiguration commandConfiguration, T entity)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var aSqlFields = new List<SqlField>();

            if (configuration.PropsInclude)
            {
                var props = typeof(T).GetProperties()
                .Where(c => c.GetCustomAttributes(typeof(NonQuerable), false).Count() == 0)
                .Select(x => new SqlField() { name = x.Name, value = entity!=null ? SqlValueUtil.GetValue(x.GetValue(entity)) : null });

                aSqlFields.AddRange(props);
            }

            if (configuration.FieldsInclude)
            {
                var fields = typeof(T).GetFields()
                .Where(c => c.GetCustomAttributes(typeof(NonQuerable), false).Count() == 0)
                .Select(x => new SqlField() { name = x.Name, value = entity != null ? SqlValueUtil.GetValue(x.GetValue(entity)) : null });

                aSqlFields.AddRange(fields);
            }

            if (commandConfiguration != null)
            {
                if (!commandConfiguration.IncludeId)
                    aSqlFields = aSqlFields
                        .Where(x => x.name.ToUpper() != commandConfiguration.PrimaryKeyTable.ToUpper())
                        .ToList();
            }
            var Elementos = aSqlFields.ToDictionary(x => x.name, y => y.value);

            return Elementos;
        }

        public static IEnumerable<string> GetKeys<T>(IClassConfiguration configuration, ICommandConfiguration commandConfiguration=null)
        {
            return GetKeysValues<T>(configuration, commandConfiguration, default(T)).Keys.ToList();
        }


        public static IEnumerable<string> GetValues<T>(IClassConfiguration Configuration, ICommandConfiguration CommandConfiguration, T Entidad)
        {
            return GetKeysValues<T>(Configuration, CommandConfiguration, Entidad).Values.ToList();
        }

        public static IEnumerable<string> GetValues<T, C>(C configuration, T entity) where C : IClassConfiguration, ICommandConfiguration => GetValues(configuration, configuration, entity);

    }
}
