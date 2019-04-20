using ExpressionToSQL.common.configuration;
using System;

namespace ExpressionToSQL.configuration
{
    public class SqlClassConfiguration : IClassConfiguration
    {
        public bool FieldsInclude { get; set; } = false;

        public bool PropsInclude { get; set; } = true;
        public SqlClassConfiguration Clone()
        {
            return this.MemberwiseClone() as SqlClassConfiguration;
        }
    }
}
