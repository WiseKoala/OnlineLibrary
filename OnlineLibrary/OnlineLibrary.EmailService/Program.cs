using System.Linq;

namespace OnlineLibrary.EmailService
{
    class Program
    {
        static void Main(string[] args)
        {
            bool verbose = false;

            if (args.Contains("verbose"))
            {
                verbose = true;
            }

            var sendEmailService = new SendEmailsHelper(verbose);

            sendEmailService.SendEmails();
        }
    }
}