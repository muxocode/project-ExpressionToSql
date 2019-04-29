using System.Collections.Generic;

namespace ExpressionToSQL.common
{
    public interface ISqlConsultant
    {
        string Delete(string table, string expression = null);
        string Insert(string table, string keys, IEnumerable<IEnumerable<string>> values, string keyTable);
        string Insert(string table, string keys, string values, string keyTable);
        string Select(string select, string table, string expression, string order = null);
        string Select(string select, string table, string expression, string row_number_name, int page, int registers, string order = null);
        string SelectGroup(string select, string table, string expression, string groupby = null);
        string Update(string table, IDictionary<string, string> FieldNameValues, string expression = null);
    }
}