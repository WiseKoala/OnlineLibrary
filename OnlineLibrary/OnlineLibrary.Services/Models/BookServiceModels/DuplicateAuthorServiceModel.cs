using System.Text.RegularExpressions;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class DuplicateAuthorServiceModel
    {
        private string _firstName;
        private string _middleName;
        private string _lastName;

        private string trimName(string name)
        {
            if (name != null)
            {
                return Regex.Replace(name, @"[^A-Za-z]", string.Empty);
            }

            return name;
        }

        public string FirstName
        {
            get { return _firstName; }

            set
            {
                _firstName = trimName(value);
            }
        }

        public string MiddleName
        {
            get { return _middleName; }

            set
            {
                _middleName = trimName(value);
            }
        }

        public string LastName
        {
            get { return _lastName; }

            set
            {
                _lastName = trimName(value);
            }
        }
    }
}