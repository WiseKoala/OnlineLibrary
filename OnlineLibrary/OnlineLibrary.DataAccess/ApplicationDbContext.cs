using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;
using System.Data.Entity;

namespace OnlineLibrary.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Role> IdentityRoles { get; set; }
        public DbSet<IdentityUserLogin> IdentityUserLogins { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanRequest> LoanRequests { get; set; }
    }
}