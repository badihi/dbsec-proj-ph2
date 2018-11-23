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
            var cmd = new NpgsqlCommand("SELECT * FROM schema_default_security WHERE table_name = @table", connection);
            cmd.Parameters.AddWithValue("table", TableName);

            var fieldNames = new StringBuilder();
            var fieldValues = new StringBuilder();

            var asls = new Dictionary<string, SecurityLevel>();
            var ails = new Dictionary<string, SecurityLevel>();
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    var aslCat = ReplaceVariables(reader.GetString(3));
                    var ailCat = ReplaceVariables(reader.GetString(5));
                    var asl = new SecurityLevel(reader.GetInt32(2), aslCat);
                    var ail = new SecurityLevel(reader.GetInt32(4), ailCat);

                    asls.Add(reader.GetString(1), asl);
                    ails.Add(reader.GetString(1), ail);
                    fieldNames.AppendFormat("{0}_asl_class, {0}_asl_cat, {0}_ail_class, {0}_ail_cat,", reader.GetString(1));
                    fieldValues.AppendFormat("{0}, '{1}', {2}, '{3}',", reader.GetInt32(2), aslCat, reader.GetInt32(4), ailCat);
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
                    fieldNames.AppendFormat("{0},", field.Key);
                    fieldValues.AppendFormat("{0},", field.Value);
                }
                else
                {
                    // Rejected
                    throw new Exception("Access denied");
                }
            }

            fieldNames.Remove(fieldNames.Length - 1, 1);
            fieldValues.Remove(fieldValues.Length - 1, 1);

            using (var insertCmd = new NpgsqlCommand())
            {
                insertCmd.Connection = connection;
                insertCmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", TableName, fieldNames.ToString(), fieldValues.ToString());
                insertCmd.ExecuteNonQuery();
            }
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
