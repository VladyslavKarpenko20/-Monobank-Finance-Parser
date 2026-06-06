using Parsing.DtoModels;
using Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing.Interface
{
    public interface IServices
    {
        Task<List<TransactionsReadDto>> GetAllTransactions();

        Task FindAndSaveTransactions(string token);
        Task<List<TransactionsReadDto>> GetByDate(DateTimeOffset from , DateTimeOffset to ,int Page , int PageSize);

        Task<AppSettings> GetAppSettings();

        Task UpdateAskPageSize(string? AskPageSize);

        Task UpdatePageSize(int? PageSize);

        Task<List<TransactionsReadDto>> GetTransactionsByPage(int Page, int PageSize);

        Task<List<TransactionsReadDto>> GetTransactionsByIncome(int Page, int PageSize);

        Task<List<TransactionsReadDto>> GetTransactionsByCosts(int Page, int PageSize);

        Task<List<TransactionsReadDto>> GetTransactionsByName(string Name, int Page, int PageSize);

        Task<MonobankToken> GetMonobankToken();

        Task UpdatemMonobankToken(MonobankToken monobankToken);

        Task<bool> CanUpdateToken();
    }
}
