using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsing.Models;

namespace Parsing.Interface
{
    public interface IRepository
    {
        Task AddTransactions(List<Transactions> transactions);

        IQueryable<Transactions> GetIQueryableTransactions();

        Task<List<Transactions>> GetAllTransactions();

        Task UpdateAppSettings(AppSettings appSettings);

        Task<AppSettings> GetAppSettings();

        Task<LimitRequest?> GetLimitRequest();

        Task UpdateLimitTime(LimitRequest limit);

        Task<MonobankToken> GetMonobankToken();

        Task UpdateMonobankToken(MonobankToken monobankToken);

    }
}
