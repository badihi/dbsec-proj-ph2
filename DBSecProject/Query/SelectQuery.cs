using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBSecProject
{
    public class SelectQuery : ReadQuery
    {
        public List<string> Fields { get; private set; }
        public string Conditions { get; set; }
        public string Function { get; set; }

        public SelectQuery(string query)
        {
            var match = Regex.Match(query, @"select\s+(sum)*\s*\(*\s*(?:((?:[A-Za-z_][A-Za-z_0-9]*|\*))\s*,\s*)*([A-Za-z_][A-Za-z_0-9]*|\*)\s*\)*\s+from\s+([A-Za-z_][A-Za-z_0-9]*)(?:\s+where\s+(.+))?", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new WrongQueryException();

            Function = match.Groups[1].Value;
            Fields = match.Groups[2].Captures
                .Cast<Capture>()
                .Select(capture => capture.Value)
                .ToList();
            Fields.Add(match.Groups[3].Value);
            TableName = match.Groups[4].Value;
            Conditions = match.Groups.Count >= 5 ? match.Groups[5].Value : null;
        }

        public override DataTable _Execute(NpgsqlConnection connection, SecurityLevel RSL, SecurityLevel RIL, int subjectId)
        {
            var encryptedDb = new EncryptedDB(connection);
            var rows = encryptedDb.Select(TableName, Fields, Conditions);

            var result = Read(rows, RSL, RIL);
            if (Function.ToLower() == "sum")
            {
                // Summation
                if (Fields.Count != 1)
                    throw new Exception("Invalid column count for aggergation");
                var newDataTable = new DataTable();
                newDataTable.Columns.Add("Sum");
                var newRow = newDataTable.NewRow();
                newRow["Sum"] = result.Rows.Cast<DataRow>().Sum(row => Convert.ToInt32(row.ItemArray[0]));
                newDataTable.Rows.Add(newRow);
                return newDataTable;
            }

            return result;
        }
    }
}
