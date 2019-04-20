using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.common.configuration
{
    public interface ITableConfiguration
    {
        string TableName { get; }
        string Schema { get; }
        string PrimaryKeyTable { get; }
    }
}
