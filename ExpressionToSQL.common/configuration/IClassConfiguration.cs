using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.common.configuration
{
    public interface IClassConfiguration
    {
        bool FieldsInclude { get; }
        bool PropsInclude { get; }
    }
}
