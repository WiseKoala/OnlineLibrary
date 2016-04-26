using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;
using System.Data.Entity;
using OnlineLibrary.DataAccess.Abstract;

namespace OnlineLibrary.DataAccess.Concrete
{
    public class LibraryDbContext : IdentityDbContext<User>, ILibraryDbContext
    {
        public LibraryDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public static LibraryDbContext Create()
        {
            return new LibraryDbContext();
        }

        public DbSet<Role> IdentityRoles { get; set; }
        public DbSet<IdentityUserLogin> IdentityUserLogins { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Loan> Loans { get; set; }
    }
}