using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.BlockIpAddress
{
    public class DAL
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["SimpleMonitorConnectionString"].ConnectionString;
        /// <summary>
        /// update the number of block hit using ado.net
        /// b = block
        /// u = unblock
        /// h = update hit block
        /// </summary>
        public static void UpdateBlockedHit(string ipAddress, string operation)
        {

            try
            {
                using (SqlConnection myConnection = new SqlConnection(_connectionString))
                {

                    using (SqlCommand sqlcommand = new SqlCommand("InsertUpdateBlockedIp", myConnection))
                    {
                        sqlcommand.CommandType = CommandType.StoredProcedure;

                        sqlcommand.Parameters.Add(new SqlParameter("@ipAddress", ipAddress));
                        sqlcommand.Parameters.Add(new SqlParameter("@operation", operation));

                        myConnection.Open();
                        sqlcommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                //errors += "yes";
            }

        }

        public static bool IsIpBlocked(string ipAddress)
        {
            using (SqlConnection myConnection = new SqlConnection(_connectionString))
            {

                using (SqlCommand sqlcommand = 
                    new SqlCommand("SELECT TOP 1 1 FROM [dbo].BlockedIp WHERE IpAddress =@ipAddress AND IsBlocked=1", myConnection))
                {
                    sqlcommand.CommandType = CommandType.Text;

                    sqlcommand.Parameters.Add(new SqlParameter("@ipAddress", ipAddress));

                    myConnection.Open();

                    object r = sqlcommand.ExecuteScalar();
                    if (r != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
