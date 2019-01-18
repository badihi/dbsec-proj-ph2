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
    public class InsertQuery : WriteQuery
    {
        public Dictionary<string, string> FieldValues { get; private set; }

        public InsertQuery(string query)
        {
            var match = Regex.Match(query, @"insert\s+into\s+([A-Za-z_][A-Za-z_0-9]*)\s*\((?:([A-Za-z_][A-Za-z_0-9]*),\s*)*([A-Za-z_][A-Za-z_0-9]*)\)\s*values\s*\(\s*(?:('[^']+'|\d+(?:\.\d+)*),\s+)+('[^']+'|\d+(?:\.\d+)*)\s*\)", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new WrongQueryException();

            if (match.Groups[2].Captures.Count != match.Groups[4].Captures.Count)
                throw new Exception("Columns and value counts do not match");

            FieldValues = match.Groups[2].Captures
                .Cast<Capture>()
                .Zip(match.Groups[4].Captures.Cast<Capture>(), (a, b) => new { Key = a, Value = b })
                .ToDictionary(obj => obj.Key.Value, obj => obj.Value.Value);

            FieldValues.Add(match.Groups[3].Value, match.Groups[5].Value);
            TableName = match.Groups[1].Value;
        }

        public override void _Execute(NpgsqlConnection connection, SecurityLevel WSL, SecurityLevel WIL)
        {
            var encryptedDb = SecureDB.GetSecureDB(connection);
            var selectedSecurity = encryptedDb.Select("schema_default_security", "table_name = '" + TableName + "'");

            var fieldValues = new Dictionary<string, string>();

            var asls = new Dictionary<string, SecurityLevel>();
            var ails = new Dictionary<string, SecurityLevel>();
            foreach (var security in selectedSecurity)
            {
                var aslCat = ReplaceVariables(security["asl_cat"]);
                var ailCat = ReplaceVariables(security["ail_cat"]);
                var asl = new SecurityLevel(Convert.ToInt32(security["asl_class"]), aslCat);
                var ail = new SecurityLevel(Convert.ToInt32(security["ail_class"]), ailCat);

                asls.Add(security["column_name"], asl);
                ails.Add(security["column_name"], ail);
                fieldValues.Add(security["column_name"] + "_asl_class", security["asl_class"]);
                fieldValues.Add(security["column_name"] + "_asl_cat", aslCat);
                fieldValues.Add(security["column_name"] + "_ail_class", security["ail_class"]);
                fieldValues.Add(security["column_name"] + "_ail_cat", ailCat);
            }
            
            // Checking write permission
            var fieldsToInsert = new Dictionary<string, string>();
            foreach (var field in FieldValues)
            {
                if (!asls.ContainsKey(field.Key))
                    throw new Exception(field.Key + " has no associated ASL");
                if (!ails.ContainsKey(field.Key))
                    throw new Exception(field.Key + " has no associated AIL");

                var asl = asls[field.Key];
                var ail = ails[field.Key];

                // Security Checking
                if (WSL <= asl && WIL >= ail)
                {
                    // Granted
                    var value = field.Value;
                    if (field.Value.StartsWith("'") && field.Value.EndsWith("'"))
                        value = field.Value.Substring(1, field.Value.Length - 2);

                    fieldValues.Add(field.Key, value);
                }
                else
                {
                    // Rejected
                    throw new Exception("Access denied");
                }
            }

            encryptedDb.InsertInto(TableName, fieldValues);
        }

        private string ReplaceVariables(string input)
        {
            var matches = Regex.Matches(input, @"@([A-Za-z_][A-Za-z_0-9]*)");
            foreach (Match match in matches)
            {
                if (!FieldValues.ContainsKey(match.Groups[1].Value))
                    throw new Exception("The field " + match.Groups[1].Value + " is required");

                input = input.Replace(match.Value, FieldValues[match.Groups[1].Value]);
            }

            return input;
        }
    }
}
