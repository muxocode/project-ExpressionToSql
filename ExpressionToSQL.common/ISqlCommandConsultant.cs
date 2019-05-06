using ExpressionToSQL.common.configuration;
using System.Collections.Generic;

namespace ExpressionToSQL.common
{
    public interface ISqlCommandConsultant
    {
        ICommandConfiguration CommandConfiguration { get; }
        string Delete(string expression = null);
        string Insert(IEnumerable<string> keys, IEnumerable<IEnumerable<string>> values);
        string Insert(IEnumerable<string> keys, IEnumerable<string> values);
        string Update(IDictionary<string, string> FieldNameValues, string expression = null);
    }
}