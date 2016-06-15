using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.DataAccess.Enums
{
    public enum BookCondition
    {
        New,
        Fine,
        [Display(Name = "Very Good")]
        VeryGood,
        Good,
        Fair,
        Poor
    }
}