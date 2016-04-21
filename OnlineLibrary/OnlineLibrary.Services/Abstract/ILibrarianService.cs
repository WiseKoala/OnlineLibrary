using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Services.Abstract
{
    public interface ILibrarianService : IService
    {
        void ApproveLoanRequest(int bookCopyId, int loanRequestId);
        void RejectLoanRequest(int loanRequestId);
        void PerformLoan(int loanId);
        IQueryable<Loan> GetApprovedLoans();
    }
}
