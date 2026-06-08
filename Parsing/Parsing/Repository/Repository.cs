using Parsing.Data;
using Parsing.Interface;
using Parsing.Models;
using Microsoft.EntityFrameworkCore;

namespace Parsing.Repository
{
    public class Repository : IRepository
    {
        private AddDbContext _context;

        public Repository(AddDbContext context)
        {
            _context = context;
        }


        public async Task AddTransactions(List<Transactions> transactions)
        {
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Transactions> GetIQueryableTransactions()
        {
            return _context.Transactions.AsQueryable();
        }

        public async Task<List<Transactions>> GetAllTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<AppSettings> GetAppSettings()
        {
            var settings = await _context.AppSettings.FirstOrDefaultAsync(x => x.Id == 1);

            if (settings == null)
            {
                settings =  new AppSettings
                {
                    Id = 1,
                    AskPageSize = false,
                    PageSize = 20
                };


                _context.AppSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return settings;

        }

        public async Task UpdateAppSettings(AppSettings appSettings)
        {
            _context.Update(appSettings);

            await _context.SaveChangesAsync();
        }

        public async Task<LimitRequest> GetLimitRequest()
        {
            var limit = await _context.LimitRequst.FirstOrDefaultAsync(x => x.Id == 1);

            if (limit == null)
            {
                limit = new LimitRequest
                {
                    Id = 1,
                    Time = null
                };

                _context.LimitRequst.Add(limit);

                await _context.SaveChangesAsync();
            }

            return limit;
        }

        public async Task UpdateLimitTime(LimitRequest limit)
        {
            _context.LimitRequst.Update(limit);

            await _context.SaveChangesAsync();
        }

        public async Task<MonobankToken> GetMonobankToken()
        {
            var token = await _context.Tokens.FirstOrDefaultAsync(x => x.Id == 1);

            if (token == null)
            {
                token = new MonobankToken
                {
                    Id = 1,
                    Token = null
                };

                _context.Tokens.Add(token);
            }

            return token;
        }

        public async Task UpdateMonobankToken(MonobankToken monobankToken)
        {
            _context.Tokens.Update(monobankToken);

            await _context.SaveChangesAsync();
        }

    }
}
