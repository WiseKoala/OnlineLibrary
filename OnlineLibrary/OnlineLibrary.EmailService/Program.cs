using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

            var adapter = new SqlDataAdapter("SELECT Email FROM Loans l JOIN AspNetUsers u ON l.UserId = u.Id WHERE l.ExpectedReturnDate = GETDATE()", connection);

            SqlCommandBuilder cmdBldr = new SqlCommandBuilder(adapter);

            adapter.Fill(loansSet, "AspNetUsers");

            for (int row = 0; row < loansSet.Tables[0].Rows.Count; row++)
            {
                string email = loansSet.Tables[0].Rows[row][0].ToString();

                SendNotification(email);
            }

            Console.ReadLine();
        }

        private static void SendNotification(string email)
        {
            using (MailMessage message = new MailMessage(ConfigurationManager.AppSettings["Email"], email))
            {
                message.Subject = ConfigurationManager.AppSettings["MessageSubject"];
                message.Body = ConfigurationManager.AppSettings["MessageBody"];

                int port = int.Parse(ConfigurationManager.AppSettings["HostPort"]);

                SmtpClient client = new SmtpClient("smtp.gmail.com", port);
                client.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Email"], 
                    ConfigurationManager.AppSettings["Password"]);

                client.Send(message);

                Console.WriteLine("Message was sent successfully!");
            }
        }
    }
}
