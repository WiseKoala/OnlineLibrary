using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace OnlineLibrary.Web.Infrastructure.CustomAttributes
{
    public class CountLimitAttribute : ValidationAttribute
    {
        private int limit
        {
            get
            {
                int a = 20;
                int.TryParse(ConfigurationManager.AppSettings["ListLimitNumber"], out a);
                return a;
            }
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;

            if (list != null)
            {
                if (list.Count < limit)
                {
                    return true;
                }
            }

            return false;
        }
    }
}