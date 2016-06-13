using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Security;
using static OnlineLibrary.Common.Infrastructure.LibraryConstants;

namespace OnlineLibrary.Common.Infrastructure
{
    public class PasswordManager
    {
        private string GeneratePassword()
        {
            int passwordLenght = Int32.Parse(ConfigurationManager.AppSettings["PasswordLength"]);
            int numberNonAlphanumeric = Int32.Parse(ConfigurationManager.AppSettings["numberNonAlphanumericChars"]);

            return Membership.GeneratePassword(passwordLenght, numberNonAlphanumeric);
        }

        private bool IsValidPassword(string password)
        {
            return !Regex.IsMatch(password, PasswordPatern);
        }

        public string CreatePasword()
        {
            string superAdminPassword;
            do
            {
                superAdminPassword = GeneratePassword();
            }
            while (!IsValidPassword(superAdminPassword));

            return superAdminPassword;
        }

    }
}
