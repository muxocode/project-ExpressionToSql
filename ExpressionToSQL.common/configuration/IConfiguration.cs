using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.common.configuration
{
    public interface IConfiguration
    {
        bool? FieldsInclude { get; }
        bool? PropsInclude { get; }
        string PrimaryKeyTable { get; }
        string TableName { get; }
    }
}
