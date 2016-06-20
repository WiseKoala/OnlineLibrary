using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.EmailService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");

            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            var connection = new SqlConnection(connectionString);

            var loansSet = new DataSet();

            var adapter = new SqlDataAdapter("SELECT Email FROM Loans l JOIN AspNetUsers u ON l.UserId = u.Id", connection);

            SqlCommandBuilder cmdBldr = new SqlCommandBuilder(adapter);

            adapter.Fill(loansSet, "AspNetUsers");

            Console.ReadLine();
        }
    }
}
