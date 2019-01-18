using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBSecProject
{
    public class SeparatedDB : SecureDB
    {
        public NpgsqlConnection Connection { get; private set; }
        private readonly string hashSecret = "P3~8(fn6f^RrRrNS";

        public SeparatedDB(NpgsqlConnection connection)
        {
            Connection = connection;
        }

        public override void DeleteFrom(string tableName, string condition)
        {
            if (!FieldMappings.ContainsKey(tableName))
                throw new Exception("Table not found: " + tableName);
            var selectedRows = Select(tableName, condition);
            if (selectedRows.Count == 0)
                return;
            if (!PrimaryKeys.ContainsKey(tableName))
                throw new Exception("No primary key for this table");
            var primaryKey = PrimaryKeys[tableName];

            var lastCurrentDb = Connection.Database;

            try
            {
                foreach (var db in FieldMappings[tableName])
                    using (var deleteCmd = new NpgsqlCommand())
                    {
                        Connection.ChangeDatabase(db.Key);

                        deleteCmd.Connection = Connection;
                        deleteCmd.CommandText = string.Format("DELETE FROM {0} WHERE " + primaryKey + " IN ({1})",
                            tableName,
                            string.Join(", ", selectedRows.Select(row => row[primaryKey]))
                        );
                        deleteCmd.ExecuteNonQuery();
                    }
            }
            finally
            {
                Connection.ChangeDatabase(lastCurrentDb);
            }
        }

        public override void InsertInto(string tableName, Dictionary<string, string> fieldValues)
        {
            if (!FieldMappings.ContainsKey(tableName))
                throw new Exception("Table not found: " + tableName);

            var lastCurrentDb = Connection.Database;

            try
            {
                foreach (var db in FieldMappings[tableName])
                {
                    List<string> names = new List<string> { };
                    List<string> values = new List<string> { };
                    var tupleForHash = new Dictionary<string, string>();
                    foreach (var field in fieldValues)
                    {
                        if (!db.Value.Contains(field.Key))
                            continue;

                        var value = field.Value;

                        if (FieldTypes.ContainsKey(field.Key))
                            switch (FieldTypes[field.Key])
                            {
                                case "date":
                                    DateTime dt;
                                    if (DateTime.TryParse(field.Value, out dt))
                                        value = dt.ToString(@"yyyy-MM-dd");
                                    else
                                        throw new Exception("Not a date: " + field.Value);
                                    break;
                                case "boolean":
                                    value = Convert.ToBoolean(field.Value) ? "true" : "false";
                                    break;
                            }
                        names.Add(field.Key);
                        values.Add(value);
                        tupleForHash.Add(field.Key, value);
                    }

                    if (!names.Any())
                        continue;

                    names.Add("hash");
                    values.Add(HashTuple(tupleForHash));

                    Connection.ChangeDatabase(db.Key);
                    using (var insertCmd = new NpgsqlCommand())
                    {
                        insertCmd.Connection = Connection;
                        insertCmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, string.Join(", ", names), "'" + string.Join("', '", values) + "'");
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                Connection.ChangeDatabase(lastCurrentDb);
            }
        }

        public override List<Dictionary<string, string>> Select(string tableName, List<string> fields, string condition)
        {
            if (!FieldMappings.ContainsKey(tableName))
                throw new Exception("Table not found: " + tableName);

            var lastCurrentDb = Connection.Database;

            try
            {
                var fieldValues = new List<Dictionary<string, string>>();
                foreach (var db in FieldMappings[tableName])
                {
                    Connection.ChangeDatabase(db.Key);

                    var newCondition = "";
                    int lastIndex = 0;
                    foreach (Match match in Regex.Matches(condition, @"([a-zA-Z_]+|'.*?'|\d+(?:\.\d+)?)\s*(=|<=|>=|>|<)\s*([a-zA-Z_]+|'.*?'|\d+(?:\.\d+)?)"))
                    {
                        newCondition += condition.Substring(lastIndex, match.Index - lastIndex);
                        var containsFields = true;
                        for (var i = 1; i <= 3; i += 2)
                        {
                            if (!float.TryParse(match.Groups[i].Value, out _) && !(match.Groups[i].Value.StartsWith("'") && match.Groups[i].Value.EndsWith("'")))
                            {
                                // It is a column name
                                if (!db.Value.Contains(match.Groups[i].Value))
                                {
                                    containsFields = false;
                                    break;
                                }
                            }
                        }

                        if (containsFields)
                        {
                            newCondition += condition.Substring(match.Index, match.Length);
                        }
                        else
                        {
                            newCondition += "1=1";
                        }

                        lastIndex = match.Index + match.Length;
                    }

                    var query = "SELECT * FROM " + tableName + " WHERE " + (string.IsNullOrWhiteSpace(newCondition) ? "1=1" : newCondition);
                    var cmd = new NpgsqlCommand(query, Connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var values = new Dictionary<string, string>();
                            var hashValue = "";
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var name = reader.GetName(i);
                                if (name == "hash")
                                {
                                    hashValue = reader.GetString(i);
                                    continue;
                                }

                                string value = "";
                                if (reader.IsDBNull(i))
                                    value = null;
                                else
                                    switch (reader.GetDataTypeName(i))
                                    {
                                        case "integer":
                                            value = reader.GetInt32(i).ToString();
                                            break;
                                        case "boolean":
                                            value = reader.GetBoolean(i) ? "true" : "false";
                                            break;
                                        case "character varying":
                                        case "character(10)":
                                            value = reader.GetString(i);
                                            break;
                                        case "date":
                                            value = reader.GetDate(i).ToString();
                                            break;
                                    }
                                values.Add(name, value);
                            }

                            if (hashValue != HashTuple(values))
                                throw new Exception("Hash is not consistent");
                            fieldValues.Add(values);
                        }

                    }

                }

                if (PrimaryKeys.ContainsKey(tableName))
                {
                    fieldValues = fieldValues
                        .GroupBy(fieldValue => fieldValue[PrimaryKeys[tableName]])
                        .Where(group => group.Count() == FieldMappings[tableName].Count)
                        .Select(group => 
                            group.SelectMany(pair => pair)
                            .GroupBy(pair => pair.Key)
                            .Select(pairGroup => pairGroup.First())
                            .ToDictionary(pair => pair.Key, pair => pair.Value))
                        .ToList();
                }

                var filteredData = new List<Dictionary<string, string>>();
                foreach (var fieldValue in fieldValues)
                {
                    if (EvaluateConditions(condition, fieldValue))
                    {
                        if (fields.Contains("*"))
                            filteredData.Add(fieldValue);
                        else
                            filteredData.Add(fieldValue.Where(item => fields.Contains(item.Key)).ToDictionary(item => item.Key, item => item.Value));
                    }
                }

                return filteredData;
            }
            finally
            {
                Connection.ChangeDatabase(lastCurrentDb);
            }
        }

        public override void UpdateSet(string tableName, Dictionary<string, string> fieldValues, string condition)
        {
            var lastCurrentDb = Connection.Database;

            try
            {
                var selectedRows = Select(tableName, condition);
                if (!PrimaryKeys.ContainsKey(tableName))
                    throw new Exception("The table has no primary key");

                var primaryKey = PrimaryKeys[tableName];

                foreach (var db in FieldMappings[tableName])
                {
                    Connection.ChangeDatabase(db.Key);

                    foreach (var row in selectedRows) {
                        var newFieldValues = new Dictionary<string, string>();
                        var tuple = new Dictionary<string, string>();
                        foreach (var field in row)
                        {
                            if (db.Value.Contains(field.Key))
                                tuple.Add(field.Key, field.Value);
                        }

                        foreach (var field in fieldValues)
                        {
                            if (db.Value.Contains(field.Key))
                            {
                                newFieldValues.Add(field.Key, field.Value);
                                tuple[field.Key] = field.Value;
                            }
                        }

                        if (row.ContainsKey("hash"))
                            row.Remove("hash");

                        if (newFieldValues.Count == 0)
                            continue;

                        newFieldValues.Add("hash", HashTuple(tuple));

                        using (var updateCmd = new NpgsqlCommand())
                        {
                            updateCmd.Connection = Connection;
                            updateCmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE " + primaryKey + " = {2}",
                                tableName,
                                string.Join(", ", newFieldValues.Select(fieldValue => fieldValue.Key + " = '" + fieldValue.Value + "'").ToArray()),
                                string.Join(", ", row[primaryKey])
                            );
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            finally
            {
                Connection.ChangeDatabase(lastCurrentDb);
            }
        }

        private string HashTuple(Dictionary<string, string> fieldValues)
        {
            var serialized = hashSecret + string.Join(",", fieldValues.OrderBy(fieldValue => fieldValue.Key).Select(fieldValue => fieldValue.Value));
            return Hash(serialized);
        }


        Dictionary<string, string> PrimaryKeys = new Dictionary<string, string>
        {
            { "doctors", "personnel_no" },
            { "nurses", "personnel_no" },
            { "patients", "application_no" },
            { "staff", "personnel_no" }
        };

        Dictionary<string, Dictionary<string, List<string>>> FieldMappings = new Dictionary<string, Dictionary<string, List<string>>>
        {
            { "doctors", new Dictionary<string, List<string>>
                {
                    { "hospital_ph2_p1", new List<string> { "personnel_no", "fname", "lname", "national_code", "personnel_no_asl_class", "personnel_no_asl_cat", "fname_asl_class", "fname_asl_cat", "lname_asl_class", "lname_asl_cat", "national_code_asl_class", "national_code_asl_cat", "subject_id", "personnel_no_ail_class", "personnel_no_ail_cat", "fname_ail_class", "fname_ail_cat", "lname_ail_class", "lname_ail_cat", "national_code_ail_class", "national_code_ail_cat" } },
                    { "hospital_ph2_p2", new List<string> { "personnel_no", "speciality", "section", "employment_date", "age", "salary", "married", "personnel_no_asl_class", "personnel_no_asl_cat", "speciality_asl_class", "speciality_asl_cat", "section_asl_class", "section_asl_cat", "employment_date_asl_class", "employment_date_asl_cat", "age_asl_class", "age_asl_cat", "salary_asl_class", "salary_asl_cat", "married_asl_class", "married_asl_cat", "personnel_no_ail_class", "personnel_no_ail_cat", "speciality_ail_class", "speciality_ail_cat", "section_ail_class", "section_ail_cat", "employment_date_ail_class", "employment_date_ail_cat", "age_ail_class", "age_ail_cat", "salary_ail_class", "salary_ail_cat", "married_ail_class", "married_ail_cat" } },
                }
            },
            { "nurses", new Dictionary<string, List<string>>
                {
                    { "hospital_ph2_p1", new List<string> { "personnel_no", "fname", "lname", "national_code", "personnel_no_asl_class", "personnel_no_asl_cat", "fname_asl_class", "fname_asl_cat", "lname_asl_class", "lname_asl_cat", "national_code_asl_class", "national_code_asl_cat", "subject_id", "personnel_no_ail_class", "personnel_no_ail_cat", "fname_ail_class", "fname_ail_cat", "lname_ail_class", "lname_ail_cat", "national_code_ail_class", "national_code_ail_cat" } },
                    { "hospital_ph2_p2", new List<string> { "personnel_no", "section", "employment_date", "age", "salary", "married", "personnel_no_asl_class", "personnel_no_asl_cat", "section_asl_class", "section_asl_cat", "employment_date_asl_class", "employment_date_asl_cat", "age_asl_class", "age_asl_cat", "salary_asl_class", "salary_asl_cat", "married_asl_class", "married_asl_cat", "personnel_no_ail_class", "personnel_no_ail_cat", "section_ail_class", "section_ail_cat", "employment_date_ail_class", "employment_date_ail_cat", "age_ail_class", "age_ail_cat", "salary_ail_class", "salary_ail_cat", "married_ail_class", "married_ail_cat" } },
                }
            },
            { "patients", new Dictionary<string, List<string>>
                {
                    { "hospital_ph2_p1", new List<string> { "application_no", "fname", "lname", "national_code", "application_no_asl_class", "application_no_asl_cat", "fname_asl_class", "fname_asl_cat", "lname_asl_class", "lname_asl_cat", "national_code_asl_class", "national_code_asl_cat", "subject_id", "application_no_ail_class", "application_no_ail_cat", "fname_ail_class", "fname_ail_cat", "lname_ail_class", "lname_ail_cat", "national_code_ail_class", "national_code_ail_cat", } },
                    { "hospital_ph2_p2", new List<string> { "application_no", "age", "male", "disease", "section", "medications", "doctor_personnel_no", "nurse_personnel_no", "application_no_asl_class", "application_no_asl_cat", "age_asl_class", "age_asl_cat", "male_asl_class", "male_asl_cat", "disease_asl_class", "disease_asl_cat", "section_asl_class", "section_asl_cat", "medications_asl_class", "medications_asl_cat", "doctor_personnel_no_asl_class", "doctor_personnel_no_asl_cat", "nurse_personnel_no_asl_class", "nurse_personnel_no_asl_cat", "application_no_ail_class", "application_no_ail_cat", "age_ail_class", "age_ail_cat", "male_ail_class", "male_ail_cat", "disease_ail_class", "disease_ail_cat", "section_ail_class", "section_ail_cat", "medications_ail_class", "medications_ail_cat", "doctor_personnel_no_ail_class", "doctor_personnel_no_ail_cat", "nurse_personnel_no_ail_class", "nurse_personnel_no_ail_cat" } },
                }
            },
            { "schema_default_security", new Dictionary<string, List<string>>
                {
                    { "hospital_ph2_p1", new List<string> { "table_name", "column_name", "asl_class", "asl_cat", "ail_class", "ail_cat" } },
                }
            },
            { "staff", new Dictionary<string, List<string>>
                {
                    { "hospital_ph2_p1", new List<string> { "personnel_no", "fname", "lname", "national_code", "personnel_no_asl_class", "personnel_no_asl_cat", "fname_asl_class", "fname_asl_cat", "lname_asl_class", "lname_asl_cat", "national_code_asl_class", "national_code_asl_cat", "subject_id", "personnel_no_ail_class", "personnel_no_ail_cat", "fname_ail_class", "fname_ail_cat", "lname_ail_class", "lname_ail_cat", "national_code_ail_class", "national_code_ail_cat" } },
                    { "hospital_ph2_p2", new List<string> { "personnel_no", "job", "employment_date", "age", "salary", "married", "personnel_no_asl_class", "personnel_no_asl_cat", "job_asl_class", "job_asl_cat", "employment_date_asl_class", "employment_date_asl_cat", "age_asl_class", "age_asl_cat", "salary_asl_class", "salary_asl_cat", "married_asl_class", "married_asl_cat", "personnel_no_ail_class", "personnel_no_ail_cat", "job_ail_class", "job_ail_cat", "employment_date_ail_class", "employment_date_ail_cat", "age_ail_class", "age_ail_cat", "salary_ail_class", "salary_ail_cat", "married_ail_class", "married_ail_cat" } },
                }
            },
            { "subjects", new Dictionary<string, List<string>>
                {
                    { "hospital_ph2_p1", new List<string> { "subject_id", "username", "password", "rsl_class", "rsl_cat", "wsl_class", "wsl_cat", "ril_class", "ril_cat", "wil_class", "wil_cat" } },
                }
            }
        };
    }
}
