using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanderasAYB
{
    internal class SqlServer
    {
        public List<Sanborn> getSanborns()
        {
            List<Sanborn> list = new List<Sanborn>();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "10.128.10.24";
            builder.UserID = "SapPsi";
            builder.Password = "kTIX3wxO8?";
            builder.InitialCatalog = "Db_Util";

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                string sql = "SELECT Id_Store, Desc_Store, Ip_Sap " +
                    "FROM Ctg_Store WHERE Id_Company = 1 AND Id_Store NOT IN (244,275,359,360,881,3000)";
                using ( SqlCommand command = new SqlCommand(sql, connection) )
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine( "Connected to server 10.128.10.24" );
                        Log.writeLineLog("Connected to server 10.128.10.24");
                        using ( SqlDataReader reader = command.ExecuteReader() )
                        {
                            while (reader.Read())
                            {
                                list.Add(new Sanborn()
                                {
                                    idSanborn = reader.GetInt32(0),
                                    nameSanborn = reader.GetString(1).Trim(),
                                    ipSanborn = reader.GetString(2).Trim()
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error connection to server 24: {1}", ex.Message);
                        Log.writeLineLog($"Error connection to server 24: {ex.Message}");
                        return null;
                    }
                }
            }
            return list;
        }
    }
}
