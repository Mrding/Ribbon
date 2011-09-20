using System;
using System.Data;
using System.Data.SqlClient;
using Luna.Common.Constants;

namespace Luna.Data
{
    public static class AdoUtil
    {
        private static readonly string ConnectionString = AppConfig.ConnectionString;

        public static object ExecuteScalar(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
                throw new ArgumentNullException("cmd");
            var conneciton = new SqlConnection(ConnectionString);
            conneciton.Open();
            using (var command = conneciton.CreateCommand())
            {
                command.CommandText = cmd;
                var result = command.ExecuteScalar();
                conneciton.Close();
                return result;
            }
        }
        public static SqlDataReader ExecuteReader(string cmd, string connection)
        {
            var conneciton = new SqlConnection(connection);
            conneciton.Open();
            using (var command = conneciton.CreateCommand())
            {
                command.CommandText = cmd;
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
        }

        public static SqlDataReader ExecuteReader(string cmd)
        {
            return ExecuteReader(cmd, ConnectionString);
        }

        public static int ExecuteNonQuery(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
                throw new ArgumentNullException("cmd");

            var conneciton = new SqlConnection(ConnectionString);
            conneciton.Open();
            using (var command = conneciton.CreateCommand())
            {
                command.CommandText = cmd;
                var result = command.ExecuteNonQuery();
                conneciton.Close();
                return result;
            }
        }
    }
}
