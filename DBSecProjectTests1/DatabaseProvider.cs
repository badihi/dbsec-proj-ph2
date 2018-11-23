using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSecProject.Tests
{
    public static class DatabaseProvider
    {
        public static NpgsqlConnection GetConnection()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AppDatabaseConnectionString"].ConnectionString;
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
