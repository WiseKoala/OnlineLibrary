﻿using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.Services.Abstract
{
    public interface ILibrarianService : IService
    {
        void ApproveLoanRequest(int bookCopyId, int loanId, int daysNumberForLateApprovedLoans);
        void RejectLoanRequest(int loanId, User librarian);
        void PerformLoan(int loanId);
        void CancelApprovedLoan(int loanId, User librarian);
        void ReturnBook(int loanId, User librarian);
        void MoveLostBookCopyToHistory(int loanId, User librarian);
        void ChangeIsLostStatus(int bookcopyId, bool isLost);
    }
}