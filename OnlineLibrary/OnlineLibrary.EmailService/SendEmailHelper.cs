using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace OnlineLibrary.EmailService
{
    class SendEmailsHelper
    {
        private bool _verbose;

        public SendEmailsHelper(bool verbose)
        {
            _verbose = verbose;
        }

        public void SendEmails()
        {
            try
            {
                WriteToConsole(ConsoleColor.White, "Start", ConsoleColor.Gray, "Starting application execution.");
                WriteToConsole(ConsoleColor.Blue, "Attempt", ConsoleColor.Gray, "Attempting to connect to the database...");

                string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

                using (var loansSet = new DataSet())
                using (var connection = new SqlConnection(connectionString))
                using (var adapter = new SqlDataAdapter(@"SELECT u.Email, b.Title
                                                        FROM Loans l
                                                        JOIN AspNetUsers u
                                                        ON l.UserId = u.Id
                                                        JOIN Books b
                                                        ON l.BookId = b.Id
                                                        WHERE l.ExpectedReturnDate = CONVERT (date, GETDATE())",
                                                        connection))
                {
                    adapter.Fill(loansSet, "AspNetUsers");

                    WriteToConsole(ConsoleColor.Green, "Success", ConsoleColor.Gray, "Connection succeeded.");

                    for (int row = 0; row < loansSet.Tables[0].Rows.Count; row++)
                    {
                        string email = loansSet.Tables[0].Rows[row][0].ToString();
                        string bookTitle = loansSet.Tables[0].Rows[row][1].ToString();

                        SendNotification(email, bookTitle);
                    }

                    if (loansSet.Tables[0].Rows.Count == 0)
                    {
                        WriteToConsole(ConsoleColor.DarkGray, "Info", ConsoleColor.Gray, "No emails were sent, because no due loans have been found.");
                    }
                }
            }
            catch (SystemException ex)
            {
                WriteToConsole(ConsoleColor.Red, "Error", ConsoleColor.Gray, string.Format("An error occurred: {0}", ex.Message));

                if (_verbose)
                {
                    WriteToConsole(ConsoleColor.Yellow, "Error details", ConsoleColor.Gray, string.Format("{0}", ex));
                }
                else
                {
                    WriteToConsole(ConsoleColor.DarkGray, "Info", ConsoleColor.Gray, "Use verbose option to see more details.");
                }
            }

            WriteToConsole(ConsoleColor.White, "Execution complete", ConsoleColor.Gray, "Press any key.");
            Console.ReadLine();
        }

        private void SendNotification(string email, string bookTitle)
        {
            using (MailMessage message = new MailMessage(ConfigurationManager.AppSettings["Email"], email))
            {
                if (_verbose)
                {
                    WriteToConsole(ConsoleColor.Blue, "Attempt", ConsoleColor.Gray, string.Format("Attempting to send a message to {0} about the book {1}...", email, bookTitle));
                }

                message.Subject = ConfigurationManager.AppSettings["MessageSubject"];
                message.Body = string.Format(ConfigurationManager.AppSettings["MessageBody"], bookTitle);

                string host = ConfigurationManager.AppSettings["HostAddress"];
                int port = int.Parse(ConfigurationManager.AppSettings["HostPort"]);

                using (SmtpClient client = new SmtpClient(host, port))
                {
                    client.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Email"],
                        ConfigurationManager.AppSettings["Password"]);

                    try
                    {
                        client.Send(message);

                        if (_verbose)
                        {
                            WriteToConsole(ConsoleColor.Green, "Success", ConsoleColor.Gray, "Message sent successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToConsole(ConsoleColor.Red, "Error", ConsoleColor.Gray, string.Format("Failed to send email to {0} about the book {1}. Error message: {2}", email, bookTitle, ex.Message));

                        if (_verbose)
                        {
                            WriteToConsole(ConsoleColor.Yellow, "Error details", ConsoleColor.Gray, string.Format("{0}", ex));
                        }
                        else
                        {
                            WriteToConsole(ConsoleColor.DarkGray, "Info", ConsoleColor.Gray, "Use verbose option to see more details.");
                        }
                    }
                }
            }
        }

        private void WriteToConsole(ConsoleColor titleColor, string title, ConsoleColor textColor, string text)
        {
            var primaryColor = Console.ForegroundColor;
            Console.ForegroundColor = titleColor;
            Console.Write(title);
            Console.Write(": ");
            Console.ForegroundColor = textColor;
            Console.Write(text);
            Console.ForegroundColor = primaryColor;
            Console.WriteLine();
        }
    }
}