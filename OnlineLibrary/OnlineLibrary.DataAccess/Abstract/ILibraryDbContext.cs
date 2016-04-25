using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.DataAccess.Abstract
{
    public interface ILibraryDbContext
    {
        DbSet<Role> IdentityRoles { get; set; }
        DbSet<IdentityUserLogin> IdentityUserLogins { get; set; }
        DbSet<Author> Authors { get; set; }
        DbSet<Book> Books { get; set; }
        DbSet<BookCopy> BookCopies { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<SubCategory> SubCategories { get; set; }
        DbSet<Loan> Loans { get; set; }
        DbSet<LoanRequest> LoanRequests { get; set; }
        IDbSet<IdentityRole> Roles { get; set; }
        IDbSet<User> Users { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
