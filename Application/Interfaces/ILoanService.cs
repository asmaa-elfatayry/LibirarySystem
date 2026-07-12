using Application.Common;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface ILoanService
{
    Task<List<LoanDto>> GetAllAsync();
    Task<List<LoanDto>> GetActiveLoansForMemberAsync(Guid memberId);
    Task<Result> BorrowAsync(Guid memberId, Guid bookId);
    Task<Result> ReturnAsync(Guid loanId);
}
