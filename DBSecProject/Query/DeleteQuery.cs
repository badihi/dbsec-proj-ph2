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
    public class DeleteQuery : WriteQuery
    {
        public string Conditions { get; set; }

        public DeleteQuery(string query)
        {
            var match = Regex.Match(query, @"delete\s+from\s+([A-Za-z_][A-Za-z_0-9]*)\s+where\s+(.+)", RegexOptions.IgnoreCase);

            if (!match.Success)
                throw new WrongQueryException();

            TableName = match.Groups[1].Value;
            Conditions = match.Groups[2].Value;
        }

        public override void _Execute(NpgsqlConnection connection, SecurityLevel WSL, SecurityLevel WIL)
        {
            var encryptedDb = SecureDB.GetSecureDB(connection);
            var selected = encryptedDb.Select(TableName, Conditions);
            
            var aslClasses = new Dictionary<string, int>();
            var aslCategories = new Dictionary<string, string>();
            var ailClasses = new Dictionary<string, int>();
            var ailCategories = new Dictionary<string, string>();
            var fieldNames = new List<string>();

            foreach (var record in selected)
            {
                foreach(var field in record)
                {
                    var name = field.Key;
                    if (name.EndsWith("_asl_class"))
                        aslClasses.Add(name.Substring(0, name.Length - 10), Convert.ToInt32(field.Value));
                    else if (name.EndsWith("_ail_class"))
                        ailClasses.Add(name.Substring(0, name.Length - 10), Convert.ToInt32(field.Value));
                    else if (name.EndsWith("_asl_cat"))
                        aslCategories.Add(name.Substring(0, name.Length - 8), field.Value);
                    else if (name.EndsWith("_ail_cat"))
                        ailCategories.Add(name.Substring(0, name.Length - 8), field.Value);
                    else
                    {
                        fieldNames.Add(name);
                    }
                }
            }

            foreach (var fieldName in fieldNames)
            {
                if (!aslClasses.ContainsKey(fieldName))
                    continue;
                var asl = new SecurityLevel(aslClasses[fieldName], aslCategories[fieldName]);
                var ail = new SecurityLevel(ailClasses[fieldName], ailCategories[fieldName]);

                // Security check
                if (!(WSL <= asl && WIL >= ail))
                {
                    throw new Exception("Access denied");
                }
            }

            encryptedDb.DeleteFrom(TableName, Conditions);
        }
    }
}
