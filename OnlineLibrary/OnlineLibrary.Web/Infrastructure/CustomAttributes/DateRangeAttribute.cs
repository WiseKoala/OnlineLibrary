using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Web.Infrastructure.CustomAttributes
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            // your validation logic
            if (Convert.ToDateTime(value) >= Convert.ToDateTime("01/01/1800") && Convert.ToDateTime(value) <= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}