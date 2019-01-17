using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

namespace DBSecProject
{
    public class EncryptedDB
    {
        private readonly string encryptionKey = "703273357538782F413F4428472B4B6250655368566D59713374367739792442";
        private readonly string encryptionIV = "782F413F4428472B4B6150645367566B";
        private readonly string indexSecret = "P3~8(fn6f^RrRrNS";
        private readonly int numberOfBuckets = 100;
        public NpgsqlConnection Connection { get; private set; }

        public EncryptedDB(NpgsqlConnection connection)
        {
            Connection = connection;
        }

        public void InsertInto(string tableName, Dictionary<string, string> fieldValues)
        {
            List<string> names = new List<string> { "data_enc", "data_hash" };
            List<string> values = new List<string> {
                EncryptTuple(fieldValues),
                HashTuple(fieldValues)
            };

            // Adding indices
            foreach (var field in fieldValues)
            {
                var index = CalculateIndex(field.Key, field.Value);
                if (index.HasValue)
                {
                    names.Add(field.Key + "_index");
                    values.Add(index.Value.ToString());
                }
            }

            using (var insertCmd = new NpgsqlCommand())
            {
                insertCmd.Connection = Connection;
                insertCmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, string.Join(", ", names), "'" + string.Join("', '", values) + "'");
                insertCmd.ExecuteNonQuery();
            }
        }

        public List<Dictionary<string, string>> Select(string tableName, string condition)
        {
            return Select(tableName, new List<string> { "*" }, condition);
        }

        public List<Dictionary<string, string>> Select(string tableName, List<string> fields, string condition)
        {
            var ret = new List<Dictionary<string, string>>();
            // Converting where conditions

            // Unequality conditions
            var matches = Regex.Matches(condition, @"([a-zA-Z_]+|\d+(?:\.\d+)?)\s*(>|<|>=|<=)\s*([a-zA-Z_]+|\d+(?:\.\d+)?)");
            var newCondition = "";
            int lastIndex = 0;
            foreach (Match match in matches)
            {
                var value1 = match.Groups[1].Value;
                var value2 = match.Groups[3].Value;
                var operation = match.Groups[2].Value;

                float valuef1, valuef2;
                var isValue1Number = float.TryParse(value1, out valuef1);
                var isValue2Number = float.TryParse(value2, out valuef2);

                if (!isValue1Number && !isValue2Number)
                    throw new Exception("Unequality conditions on two fields is not supported");
                else if (isValue2Number && isValue1Number)
                {
                    newCondition += " " + value1 + operation + value2 + " ";
                    continue;
                }

                var value = isValue1Number ? (int)valuef1 : (int)valuef2;
                var fieldName = isValue1Number ? value2 : value1;

                var fieldType = FieldTypes.ContainsKey(fieldName) ? FieldTypes[fieldName] : "";
                if (fieldType != "integer")
                    throw new Exception("Unequality condition is only accpeted for integers");

                newCondition += condition.Substring(lastIndex, match.Index - lastIndex) + fieldName + "_index in (";

                var maxValue = MaxValues.ContainsKey(fieldName) ? MaxValues[fieldName] : 0;
                var indices = new List<int>();
                // Greater than
                if (isValue2Number && operation.Contains(">") || isValue1Number && operation.Contains("<"))
                {
                    for (
                            var i = (value * numberOfBuckets / maxValue) * maxValue / numberOfBuckets; 
                            i < maxValue; 
                            i += maxValue / numberOfBuckets
                        )
                    {
                        var index = CalculateIndex(fieldName, i.ToString());
                        if (!index.HasValue)
                            throw new Exception("Field is not known: " + fieldName);
                        indices.Add(index.Value);
                    }
                }

                // Less than
                if (isValue1Number && operation.Contains(">") || isValue2Number && operation.Contains("<"))
                {
                    for (
                            var i = 0;
                            i <= (value * numberOfBuckets / maxValue) * maxValue / numberOfBuckets;
                            i += maxValue / numberOfBuckets
                        )
                    {
                        var index = CalculateIndex(fieldName, i.ToString());
                        if (!index.HasValue)
                            throw new Exception("Field is not known: " + fieldName);
                        indices.Add(index.Value);
                    }
                }

                newCondition += string.Join(", ", indices) + ")";
                lastIndex = match.Index + match.Length;
            }
            newCondition += condition.Substring(lastIndex);

            // Equality
            var newNewCondition = "";
            lastIndex = 0;
            foreach (Match match in Regex.Matches(newCondition, @"([a-zA-Z_]+|'.*?'|\d+(?:\.\d+)?)\s*=\s*([a-zA-Z_]+|'.*?'|\d+(?:\.\d+)?)"))
            {
                newNewCondition += newCondition.Substring(lastIndex, match.Index - lastIndex);
                bool isValue1Field = !float.TryParse(match.Groups[1].Value, out _) && (!match.Groups[1].Value.StartsWith("'") || !match.Groups[1].Value.EndsWith("'"));
                bool isValue2Field = !float.TryParse(match.Groups[2].Value, out _) && (!match.Groups[2].Value.StartsWith("'") || !match.Groups[2].Value.EndsWith("'"));

                if (!isValue1Field && !isValue2Field)
                {
                    newNewCondition += match.Groups[1].Value + "_index" + "=" + match.Groups[2].Value + "_index";
                    continue;
                }

                var fieldName = isValue1Field ? match.Groups[1].Value : match.Groups[2].Value;
                if (!FieldTypes.ContainsKey(fieldName))
                    throw new Exception("Unknown field name: " + fieldName);

                if (isValue1Field)
                    newNewCondition += match.Groups[1].Value + "_index";
                else
                {
                    var value = match.Groups[1].Value;
                    if (value.StartsWith("'"))
                        value = value.Substring(1);
                    if (value.EndsWith("'"))
                        value = value.Substring(0, value.Length - 1);
                    newNewCondition += CalculateIndex(fieldName, value);
                }

                newNewCondition += "=";

                if (isValue2Field)
                    newNewCondition += match.Groups[2].Value + "_index";
                else
                {
                    var value = match.Groups[2].Value;
                    if (value.StartsWith("'"))
                        value = value.Substring(1);
                    if (value.EndsWith("'"))
                        value = value.Substring(0, value.Length - 1);
                    newNewCondition += CalculateIndex(fieldName, value);
                }

                lastIndex = match.Index + match.Length;
            }
            newNewCondition += newCondition.Substring(lastIndex);

            var query = "SELECT id, data_enc, data_hash FROM " + tableName + " WHERE " + (string.IsNullOrWhiteSpace(newNewCondition) ? "1=1" : newNewCondition);
            var cmd = new NpgsqlCommand(query, Connection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var encryptedData = reader.GetString(1);
                    var hashedData = reader.GetString(2);
                    var data = DecryptTuple(encryptedData);

                    // Checking integrity
                    if (hashedData != HashTuple(data))
                        throw new Exception("Hash is not consistent with the data");

                    data["id"] = id.ToString();

                    var evalCondition = "";
                    lastIndex = 0;
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

                    if (EvaluateLogicalExpression(evalCondition))
                    {
                        if (fields.Contains("*"))
                            ret.Add(data);
                        else
                            ret.Add(data.Where(item => fields.Contains(item.Key)).ToDictionary(item => item.Key, item => item.Value));
                    }
                }
            }

            return ret;
        }

        public void UpdateSet(string tableName, Dictionary<string, string> fieldValues, string condition)
        {
            var selectedRows = Select(tableName, condition);
            foreach (var row in selectedRows)
            {
                foreach (var fieldValue in fieldValues)
                {
                    if (!row.ContainsKey(fieldValue.Key))
                        throw new Exception("Field not found: " + fieldValue.Key);
                    row[fieldValue.Key] = fieldValue.Value;
                }

                Dictionary<string, string> newFieldValues = new Dictionary<string, string>
                {
                    { "data_enc", EncryptTuple(row) },
                    { "data_hash", HashTuple(row) }
                };

                // Adding indices
                foreach (var field in row)
                {
                    var index = CalculateIndex(field.Key, field.Value);
                    if (index.HasValue)
                    {
                        newFieldValues.Add(field.Key + "_index", index.Value.ToString());
                    }
                }

                using (var updateCmd = new NpgsqlCommand())
                {
                    updateCmd.Connection = Connection;
                    updateCmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE id = {2}",
                        tableName,
                        string.Join(", ", newFieldValues.Select(fieldValue => fieldValue.Key + " = '" + fieldValue.Value + "'").ToArray()),
                        row["id"]
                    );
                    updateCmd.ExecuteNonQuery();
                }
            }
            
        }

        public void DeleteFrom(string tableName, string condition)
        {
            var selectedRows = Select(tableName, condition);
            using (var deleteCmd = new NpgsqlCommand())
            {
                deleteCmd.Connection = Connection;
                deleteCmd.CommandText = string.Format("DELETE FROM {0} WHERE id IN ({1})",
                    tableName,
                    string.Join(", ", selectedRows.Select(row => row["id"]))
                );
                deleteCmd.ExecuteNonQuery();
            }
        }

        private string HashTuple(Dictionary<string, string> fieldValues)
        {
            var serialized = string.Join(",", fieldValues.OrderBy(fieldValue => fieldValue.Key).Select(fieldValue => fieldValue.Value));
            return Hash(serialized);
        }

        private string Hash(string input)
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

        public static bool EvaluateLogicalExpression(string logicalExpression)
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

        private int? CalculateIndex(string fieldName, string value)
        {
            if (!FieldTypes.ContainsKey(fieldName))
                return null;
            var type = FieldTypes[fieldName];

            SHA1Managed sha1 = new SHA1Managed();
            string index = "";
            switch (type)
            {
                case "integer":
                    var maxValue = 0;
                    if (MaxValues.ContainsKey(fieldName))
                        maxValue = MaxValues[fieldName];

                    if (string.IsNullOrEmpty(value))
                    {
                        index = "0";
                    } else {
                        var intValue = Convert.ToInt32(value);
                        index = (intValue * numberOfBuckets / maxValue).ToString();
                    }
                    break;
                case "boolean":
                    if (string.IsNullOrEmpty(value))
                        index = "2";
                    else
                    {
                        var boolValue = Convert.ToBoolean(value);
                        index = boolValue ? "1" : "0";
                    }
                    break;
                case "date":
                    if (string.IsNullOrEmpty(value))
                        index = "0";
                    else
                    {
                        var dateValue = Convert.ToDateTime(value);
                        index = dateValue.Ticks.ToString();
                    }
                    break;
                case "character":
                default:
                    index = value == null ? "" : value;
                    break;
            }

            return (int)(BitConverter.ToUInt32(sha1.ComputeHash(Encoding.UTF8.GetBytes(index + fieldName + indexSecret)), 0) / 2);
        }

        private string EncryptTuple(Dictionary<string, string> fieldValues)
        {
            var serialized = JsonConvert.SerializeObject(fieldValues);
            var key = StringToByteArray(encryptionKey);
            var iv = StringToByteArray(encryptionIV);
            return Convert.ToBase64String(EncryptStringToBytes(serialized, key, iv));
        }

        private Dictionary<string, string> DecryptTuple(string encrypted)
        {
            var key = StringToByteArray(encryptionKey);
            var iv = StringToByteArray(encryptionIV);
            var data = DecryptStringFromBytes(Convert.FromBase64String(encrypted), key, iv);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;

        }

        private string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        private Dictionary<string, int> MaxValues = new Dictionary<string, int>
        {
            { "personnel_no", 10000000 },
            { "age", 100 },
            { "salary", 10000000 },
            { "subject_id", 100000 },
            { "application_no", 100000 },
            { "doctor_personnel_no", 100000 },
            { "nurse_personnel_no", 100000 }
        };

        private Dictionary<string, string> FieldTypes = new Dictionary<string, string>
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
