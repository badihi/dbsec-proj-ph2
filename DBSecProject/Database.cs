using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSecProject
{
    public class Database
    {
        private NpgsqlConnection connection;
        public Subject Subject { get; private set; } = null;
        public Database(string connectionString)
        {
            connection = new NpgsqlConnection(connectionString);
            /*using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                    cmd.Parameters.AddWithValue("p", "Hello world");
                    cmd.ExecuteNonQuery();
                }

                // Retrieve all rows
                using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        Console.WriteLine(reader.GetString(0));
            }*/
        }

        public void Connect()
        {
            connection.Open();
        }

        public void Login(string username, string password)
        {
            var encryptedDb = SecureDB.GetSecureDB(connection);
            var rows = encryptedDb.Select("subjects", string.Format("username = '{0}' AND password = '{1}'", username, password));

            if (!rows.Any())
            {
                throw new Exception("Username or password is not correct");
            }

            var subject = rows[0];
            Subject = new Subject
            {
                Id = Convert.ToInt32(subject["subject_id"]),
                Username = subject["username"],
                Password = subject["password"],
                RSL = new SecurityLevel(Convert.ToInt32(subject["rsl_class"]), subject["rsl_cat"]),
                WSL = new SecurityLevel(Convert.ToInt32(subject["wsl_class"]), subject["wsl_cat"]),
                RIL = new SecurityLevel(Convert.ToInt32(subject["ril_class"]), subject["ril_cat"]),
                WIL = new SecurityLevel(Convert.ToInt32(subject["wil_class"]), subject["wil_cat"]),
            };
        }

        public QueryResult ExecuteQuery(string queryString)
        {
            Query query;

            try
            {
                query = Query.Parse(queryString);
            }
            catch (Exception ex)
            {
                return new QueryResult
                {
                    Error = ex.Message,
                    Status = "An error occured",
                    Type = QueryResultType.Text
                };
            }

            return query.Execute(connection, Subject.RSL, Subject.WSL, Subject.RIL, Subject.WIL, Subject.Id);
        }

        public bool IsAuthenticated()
        {
            return Subject != null;
        }
    }
}
