using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.Services.Abstract
{
    public interface ILibrarianService : IService
    {
        void ApproveLoanRequest(int bookCopyId, int loanId, int daysNumberForLateApprovedLoans);

        void RejectLoanRequest(int loanId, User librarian);

        void PerformLoan(int loanId);

        void CancelApprovedLoan(int loanId, User librarian);

        void ReturnBook(int loanId, User librarian /*, BookCondition finalBookCondition*/);

        void MoveLostBookCopyToHistory(int loanId, User librarian);
    }
}