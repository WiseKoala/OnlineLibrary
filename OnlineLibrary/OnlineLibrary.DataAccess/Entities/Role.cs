using Microsoft.AspNet.Identity.EntityFramework;

namespace OnlineLibrary.DataAccess.Entities
{
    public class Role : IdentityRole
    {
        public Role() : base()
        {
        }

        public Role(string name) : base(name)
        {
        }
    }
}