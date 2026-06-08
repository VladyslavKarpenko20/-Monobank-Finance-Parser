using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Parsing.DtoModels;
using Parsing.Interface;
using Parsing.Migrations;
using Parsing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

namespace Parsing.Services
{
    public class Services : IServices
    {
        private IRepository _repository;
        private HttpClient _httpClient;

        public Services(IRepository repository) 
        {
            _repository = repository;
            _httpClient = new HttpClient();
        }

        public async Task<List<TransactionsReadDto>> GetAllTransactions()
        {
            var listT = await _repository.GetAllTransactions();


             var res = listT.Select(x => new TransactionsReadDto
             {
                 Id = x.Id,
                CurentName = x.CurentName,
                Descriptions = x.Descriptions,
                Ammount = x.Ammount,
                Time = x.Time,
                Balance = x.Balance,
            }).ToList();

            return res;
        }

        public async Task<List<TransactionsReadDto>> GetByDate(DateTimeOffset from, DateTimeOffset to, int Page, int PageSize)
        {
            if (from > to)
                throw new Exception("Стартова дата не може бути більшою за кінцеву");

            return await Mapping(t => t.Time >= from && t.Time <= to, Page, PageSize);

        }

        public async Task FindAndSaveTransactions(string token)
        {
            var limitTime = await _repository.GetLimitRequest();

            var currentTime = DateTimeOffset.UtcNow;

            if (limitTime.Time > currentTime)
            {
                TimeSpan timeSpan = (limitTime.Time - currentTime).Value;

                var timeLeft = timeSpan.TotalSeconds;

                throw new Exception($"Запити можна робити раз в 70 секунд залишилось {(int)timeLeft}");
            }
            else
            {
                limitTime.Time = currentTime.AddSeconds(70);
                await _repository.UpdateLimitTime(limitTime);
            }

                    


            _httpClient.DefaultRequestHeaders.Add("X-Token",token);

            var toTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var fromTime = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeSeconds();

            string url = $"https://api.monobank.ua/personal/statement/0/{fromTime}/{toTime}";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            var list = JsonSerializer.Deserialize<List<MonoBankApi>>(jsonString);

            if (list == null || !list.Any())
            {
                Console.WriteLine("Немає транзакцій");
                return;
            }


            var transactions = new List<Transactions>();

            var allTransactions = (await _repository.GetAllTransactions())
                .Select(t => t.TransactionId)
                .ToHashSet();

            foreach(var res in list)
            {
                if (allTransactions.Contains(res.id))
                    continue;

                var transaction = new Transactions
                {
                    TransactionId = res.id,
                    CurentName = res.counterName ?? "Імя пусте",
                    Descriptions = res.description ?? "Без опису",
                    Ammount = res.amount / 100m,
                    Time = DateTimeOffset.FromUnixTimeSeconds(res.time).ToUniversalTime(),
                    Balance = res.balance

                };

                transactions.Add(transaction);
                
            }

            Console.WriteLine($"Нових транзакція знайдено та збережено:  {transactions.Count}");

            await _repository.AddTransactions(transactions);

        }

        public async Task<List<TransactionsReadDto>> GetTransactionsByPage(int Page, int PageSize)
        {
            if (Page <= 0 || PageSize <= 0 || PageSize > 10000)
                throw new Exception("Не правильно вказані данні ");

            return await _repository.GetIQueryableTransactions()
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new TransactionsReadDto
                {
                    Id = x.Id,
                    CurentName = x.CurentName,
                    Descriptions = x.Descriptions,
                    Ammount = x.Ammount,
                    Balance = x.Balance,
                    Time = x.Time
                }).ToListAsync();
        }

        public async Task<AppSettings> GetAppSettings()
        {
            return await _repository.GetAppSettings();
        }

        public async Task UpdatePageSize(int? PageSize)
        {
            if (PageSize <= 0 || PageSize > 1000 )
            {
                throw new Exception("Не правильно вказані данні ");
            }
            var settings = await _repository.GetAppSettings();

            if (settings == null)
                settings = new AppSettings();

            if (PageSize != null)
            {
                settings.PageSize = PageSize.Value;
                Console.WriteLine("Розмір сторінки змінено");
            }
                await _repository.UpdateAppSettings(settings);
        }
        public async Task UpdateAskPageSize(string? AskPageSize)
        {
            if (string.IsNullOrEmpty(AskPageSize))
            {
                Console.WriteLine("AskPageSize = null");
                return;
            }
            if (AskPageSize.Trim().ToLower() != "так" && AskPageSize.Trim().ToLower() != "ні")
            {
                throw new Exception("Не правильно вказані данні ");
            }

            var settings = await _repository.GetAppSettings();

            if (settings == null)
                settings = new AppSettings();

            if (AskPageSize != null)
            {
                if (AskPageSize.ToLower().Trim() == "так")
                    settings.AskPageSize = true;
                else
                    settings.AskPageSize = false;
            }

            Console.WriteLine("Налаштування режиму запиту змінено");
            await _repository.UpdateAppSettings(settings);
        }

        public async Task<List<TransactionsReadDto>> GetTransactionsByIncome(int Page , int PageSize)
        {
            return await Mapping(x => x.Ammount > 0, Page, PageSize);
        }

        public async Task<List<TransactionsReadDto>> GetTransactionsByCosts(int Page, int PageSize)
        {
            return await Mapping(x => x.Ammount < 0, Page, PageSize);
        }

        public async Task<List<TransactionsReadDto>> GetTransactionsByName(string Name,int Page, int PageSize)
        {
            if (Name == null || Name.Count() < 2)
                throw new Exception("Не правильно вказані данні ");


            return await Mapping(x => x.CurentName != null && x.CurentName.ToLower() == Name.ToLower(), Page, PageSize);
        }

        private async Task<List<TransactionsReadDto>> Mapping(Expression<Func<Transactions, bool>> expression, int Page , int PageSize)
        {

            if (Page <= 0 || PageSize <= 0 || PageSize > 10000)
            {
                throw new Exception("Не правильно вказані данні ");
            }

            return await _repository.GetIQueryableTransactions()
                .Where(expression)
                .OrderByDescending(x => x.Ammount)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new TransactionsReadDto
                {
                    Id = x.Id,
                    CurentName = x.CurentName,
                    Descriptions = x.Descriptions,
                    Ammount = x.Ammount,
                    Balance = x.Balance,
                    Time = x.Time

                }).ToListAsync();
        }

        public async Task<MonobankToken> GetMonobankToken()
        {
            return await _repository.GetMonobankToken();
        }

        public async Task UpdatemMonobankToken(MonobankToken monobankToken)
        {

            if (string.IsNullOrWhiteSpace(monobankToken.Token))
                throw new Exception("Токен введено не коректно");

            await _repository.UpdateMonobankToken(monobankToken);
        }

        public async Task<bool> CanUpdateToken()
        {
            var time = await _repository.GetLimitRequest();

            if (time.Time < DateTimeOffset.UtcNow)
                return true;

            else return false;
        }
    }
}
