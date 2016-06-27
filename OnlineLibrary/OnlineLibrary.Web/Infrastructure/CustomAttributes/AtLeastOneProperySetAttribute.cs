using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Web.Infrastructure.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AtLeastOnePropertySetAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                // Get the information about the class.
                var typeInfo = value.GetType();

                // Get the properties of the class.
                var propertyInfo = typeInfo.GetProperties();

                foreach (var property in propertyInfo)
                {
                    if (property.GetValue(value, null) != null)
                    {
                        if (!string.IsNullOrEmpty(property.ToString()))
                        {
                            // There is at least one property with a value.
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}