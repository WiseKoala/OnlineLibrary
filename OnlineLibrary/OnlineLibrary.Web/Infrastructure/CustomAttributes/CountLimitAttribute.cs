using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace OnlineLibrary.Web.Infrastructure.CustomAttributes
{
    public class CountLimitAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IList;
            var limit = int.Parse(ConfigurationManager.AppSettings["ListLimitNumber"]);

            if (list != null && list.Count < limit)
            {
                return true;
            }

            return false;
        }
    }
}