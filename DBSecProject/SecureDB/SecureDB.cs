using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBSecProject
{
    public enum SecureDBType { EncryptedDB, SeparatedDB }
    public abstract class SecureDB
    {
        public static SecureDBType Type { get; set; } = SecureDBType.EncryptedDB;
        public static SecureDB GetSecureDB(NpgsqlConnection connection)
        {
            switch (Type)
            {
                case SecureDBType.EncryptedDB:
                    return new EncryptedDB(connection);
                default:
                case SecureDBType.SeparatedDB:
                    return new SeparatedDB(connection);
            }
        }

        public abstract void InsertInto(string tableName, Dictionary<string, string> fieldValues);
        public List<Dictionary<string, string>> Select(string tableName, string condition)
        {
            return Select(tableName, new List<string> { "*" }, condition);
        }
        public abstract List<Dictionary<string, string>> Select(string tableName, List<string> fields, string condition);
        public abstract void UpdateSet(string tableName, Dictionary<string, string> fieldValues, string condition);
        public abstract void DeleteFrom(string tableName, string condition);

        protected bool EvaluateConditions(string condition, Dictionary<string, string> data)
        {
            var evalCondition = "";
            int lastIndex = 0;
            foreach (Match match in Regex.Matches(condition, @"([a-zA-Z_]+|'.*?'|\d+(?:\.\d+)?)\s*(=|<=|>=|>|<)\s*([a-zA-Z_]+|'.*?'|\d+(?:\.\d+)?)"))
            {
                evalCondition += condition.Substring(lastIndex, match.Index - lastIndex);

                var values = new string[4];
                var fieldNames = new string[4];
                for (var i = 1; i <= 3; i += 2)
                {
                    if (float.TryParse(match.Groups[i].Value, out _))
                        values[i] = match.Groups[i].Value;
                    else if (match.Groups[i].Value.StartsWith("'") && match.Groups[i].Value.EndsWith("'"))
                    {
                        values[i] = match.Groups[i].Value.Substring(1, match.Groups[i].Value.Length - 2);
                    }
                    else
                    {
                        if (!data.ContainsKey(match.Groups[i].Value))
                            throw new Exception("Unknown field: " + match.Groups[i].Value);

                        values[i] = data[match.Groups[i].Value];
                        fieldNames[i] = match.Groups[i].Value;
                    }
                }

                var result = false;
                if (fieldNames[1] == null && fieldNames[3] == null || fieldNames[1] != null && fieldNames[3] != null)
                    result = values[1] == values[3];
                else
                {
                    var fieldType = fieldNames[1] != null ? FieldTypes[fieldNames[1]] : FieldTypes[fieldNames[3]];
                    switch (fieldType)
                    {
                        case "integer":
                            if (values[1] == "" || values[3] == "")
                            {
                                result = false;
                                break;
                            }

                            var intVal1 = Convert.ToDouble(values[1]);
                            var intVal2 = Convert.ToDouble(values[3]);
                            switch (match.Groups[2].Value)
                            {
                                case "=":
                                    result = intVal1 == intVal2;
                                    break;
                                case "<=":
                                    result = intVal1 <= intVal2;
                                    break;
                                case ">=":
                                    result = intVal1 >= intVal2;
                                    break;
                                case "<":
                                    result = intVal1 < intVal2;
                                    break;
                                case ">":
                                    result = intVal1 > intVal2;
                                    break;
                            }
                            break;
                        case "date":
                            if (values[1] == "" || values[3] == "")
                            {
                                result = false;
                                break;
                            }
                            var dateVal1 = Convert.ToDateTime(values[1]);
                            var dateVal2 = Convert.ToDateTime(values[3]);
                            switch (match.Groups[2].Value)
                            {
                                case "=":
                                    result = dateVal1 == dateVal2;
                                    break;
                                case "<=":
                                    result = dateVal1 <= dateVal2;
                                    break;
                                case ">=":
                                    result = dateVal1 >= dateVal2;
                                    break;
                                case "<":
                                    result = dateVal1 < dateVal2;
                                    break;
                                case ">":
                                    result = dateVal1 > dateVal2;
                                    break;
                            }
                            break;
                        case "boolean":
                            if (values[1] == "" || values[3] == "")
                            {
                                result = false;
                                break;
                            }
                            var boolVal1 = Convert.ToBoolean(values[1]);
                            var boolVal2 = Convert.ToBoolean(values[3]);
                            switch (match.Groups[2].Value)
                            {
                                case "=":
                                    result = boolVal1 == boolVal2;
                                    break;
                            }
                            break;
                        default:
                            switch (match.Groups[2].Value)
                            {
                                case "=":
                                    result = values[1] == values[3];
                                    break;
                            }
                            break;
                    }
                }

                evalCondition += result ? " True " : " False ";

                lastIndex = match.Index + match.Length;
            }
            evalCondition += condition.Substring(lastIndex);

            return EvaluateLogicalExpression(evalCondition);
        }

        private static bool EvaluateLogicalExpression(string logicalExpression)
        {
            if (string.IsNullOrWhiteSpace(logicalExpression))
                return true;

            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("", typeof(bool));
            table.Columns[0].Expression = logicalExpression;

            System.Data.DataRow r = table.NewRow();
            table.Rows.Add(r);
            bool result = (Boolean)r[0];
            return result;
        }

        protected string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        protected Dictionary<string, string> FieldTypes = new Dictionary<string, string>
        {
            { "personnel_no", "integer" },
            { "fname", "character" },
            { "lname", "character" },
            { "national_code", "character" },
            { "speciality", "character" },
            { "section", "character" },
            { "employment_date", "date" },
            { "age", "integer" },
            { "salary", "integer" },
            { "married", "boolean" },
            { "subject_id", "integer" },
            { "application_no", "integer" },
            { "male", "boolean" },
            { "disease", "character" },
            { "medications", "character" },
            { "doctor_personnel_no", "integer" },
            { "nurse_personnel_no", "integer" },
            { "job", "character" },
            { "username", "character" },
            { "password", "character" },
            { "table_name", "character" },
            { "column_name", "character" }
        };

    }
}
