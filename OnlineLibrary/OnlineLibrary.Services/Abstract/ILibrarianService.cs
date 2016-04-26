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
        bool TryApproveLoanRequest(int bookCopyId, int loanId);
        void RejectLoanRequest(int loanId);
        void PerformLoan(int loanId);
    }
}
