using Microsoft.Data.SqlClient;

namespace BusinessLayer
{
    // Central place for the connection string so every entity class uses the same one.
    internal static class Database
    {
        // Connection to the pre-installed DokterspraktijkDB on the local SQL Express instance.
        internal const string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=DokterspraktijkDB;Integrated Security=True;TrustServerCertificate=True;";

        // Factory method — callers open/close via using or try/finally themselves.
        internal static SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
