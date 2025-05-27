using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace EvselAtikTakip.Classes
{
    public class DatabaseHelper
    {
        private static string connectionString = "Server=ŞEYMA\\SQLEXPRESS;Database=atiktakip_;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
        public static DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }
            return dt;
        }
        public static int ExecuteNonQuery(string commandText)
        {
            using(SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(commandText, conn);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
