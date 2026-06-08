using Microsoft.EntityFrameworkCore.Update.Internal;
using Npgsql.Internal;
using Parsing.DtoModels;
using Parsing.Interface;
using Parsing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parsing.Endpoints
{
    public class Endpoint
    {
        private IServices _services;

        public Endpoint(IServices services)
        {
            _services = services;
        }


        public async Task RunAsync()
        {

            Console.WriteLine("----MonoBank Parser Date----");
            Console.WriteLine("Вiтаю в парсерi данних\n");

            var token = await _services.GetMonobankToken();

            if (string.IsNullOrEmpty(token.Token))
                Console.WriteLine("\nУвага ви не вказали свій Monobank токен або ми втратили його!\n Через це ми не можемо оновлювати ваші данні в фоні!\n Перейдіт в розідл \"Налаштування\" щоб вказати токен");

            while (true)
            {
                
                
                Console.WriteLine("Оберiть дiю: \n1)Показати всi транзакцiї \n2)Отримати та зберiгти транзакцiї \n3)Показати транзакцiї за датою \n4)Показ транзакцій за фільтрами \n5)Налаштування \n6)Вийти");

                var settings = await _services.GetAppSettings();
                int numberChoise = ReadNumber();

                try
                {
                    switch (numberChoise)
                    {

                        case 1:
                            Console.WriteLine("Оберiть сторiнку");
                            int Page = ReadNumber();

                            int PageSize;

                            if (settings.AskPageSize == true)
                            {
                                Console.WriteLine("Оберiть розмiр сторiнки");
                                PageSize = ReadNumber();
                            }
                            else
                                PageSize = settings.PageSize;

                            DisplayTransactions(await _services.GetTransactionsByPage(Page, PageSize));
                            break;

                        case 2:
                            string? MonobankToken;

                            if (token == null)
                            {
                                Console.WriteLine("Помилка ми не змогли витягнути токен з бази данних ");
                                continue;
                            }
                            else if (token.Token == null)
                            {
                                Console.WriteLine("Уведiть ваш токен MonoBank");
                                MonobankToken = Console.ReadLine();
                            }
                            else
                                MonobankToken = token.Token;


                            if (MonobankToken != null)
                                await _services.FindAndSaveTransactions(MonobankToken.Trim());

                            break;

                        case 3:
                            Console.WriteLine("Початкова дата (приклад 2025-05-26)");
                            string? from = Console.ReadLine();
                            Console.WriteLine("Дата закiнчення (приклад 2026-06-03)");
                            string? to = Console.ReadLine();

                            Console.WriteLine("Оберіть сторінку");


                            int PageForDate = ReadNumber();
                            int PageSizeForDate;

                            if (settings.AskPageSize == true)
                            {
                                Console.WriteLine("Оберіть розмір сторінки ");

                                PageSizeForDate = ReadNumber();
                            }
                            else
                                PageSizeForDate = settings.PageSize;

                            string format = "yyyy-MM-dd";
                            var culture = System.Globalization.CultureInfo.InvariantCulture;
                            var style = System.Globalization.DateTimeStyles.AssumeUniversal;

                            if (DateTimeOffset.TryParseExact(from, format, culture, style, out DateTimeOffset resultFrom) && DateTimeOffset.TryParseExact(to, format, culture, style, out DateTimeOffset resultTo))
                            {
                                resultTo = resultTo.AddDays(1);

                                DisplayTransactions(await _services.GetByDate(resultFrom.ToUniversalTime(), resultTo.ToUniversalTime(), PageForDate, PageSizeForDate));

                            }
                            else
                            {
                                throw new Exception("Не правильно введена дата!");
                            }
                            break;


                        case 4:
                            Console.WriteLine("Оберіть за якім фільтром бажаєте отримати данні: \n1)За Доходом \n2)За Витратами \n3)За іменем \n4)Повернутись в меню");

                            int numberFilter = ReadNumber();

                            Console.WriteLine("Оберіть сторінку");
                            int FilterPage = ReadNumber();

                            int FilterPageSize;

                            if (settings.AskPageSize == true)
                            {
                                Console.WriteLine("Оберіть розмір сторінки ");
                                FilterPageSize = ReadNumber();
                            }
                            else
                                FilterPageSize = settings.PageSize;

                            switch (numberFilter)
                            {
                                case 1:
                                    Console.WriteLine("Список транзакцій за доходом");
                                    DisplayTransactions(await _services.GetTransactionsByIncome(FilterPage, FilterPageSize));
                                    continue;

                                case 2:
                                    Console.WriteLine("Список транзакцій за витратами");
                                    DisplayTransactions(await _services.GetTransactionsByCosts(FilterPage, FilterPageSize));
                                    continue;

                                case 3:
                                    Console.WriteLine("Уведіть імя ");
                                    string? Name = Console.ReadLine();

                                    if (Name == null)
                                    {
                                        Console.WriteLine("Імя не може бути пустим ");
                                        continue;
                                    }
                                    Console.WriteLine($"Список транзакцій за імям {Name}");
                                    DisplayTransactions(await _services.GetTransactionsByName(Name, FilterPage, FilterPageSize));
                                    continue;

                                case 4:
                                    break;
                            }




                            break;

                        case 5:
                            Console.WriteLine("~~Налаштування~~");
                            var setting = await _services.GetAppSettings();
                            string AskPageSizes = setting.AskPageSize == true ? "Так" : "Ні";

                            Console.WriteLine($"Розмір сторінки = {setting.PageSize}\n Запитувати розмір сторінки |{AskPageSizes} ");

                            Console.WriteLine("Оберіть дію: \n1)Змінити розмір сторінки \n2)Змініти режим запиту розміру сторінки \n3)Записати Monobank токен \n4)Повернутись в меню");

                            int numberSettings = ReadNumber();

                            switch (numberSettings)
                            {

                                case 1:
                                    Console.WriteLine("Оберіть розмір сторінки");

                                    if (int.TryParse(Console.ReadLine(), out int result))
                                    {
                                        await _services.UpdatePageSize(result);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Ви ввели не правильне значення");
                                        continue;
                                    }
                                    break;

                                case 2:
                                    Console.WriteLine("Вкажіть режим запиту розміру сторінки (Так/Ні)");
                                    string? AskPageSize = Console.ReadLine();
                                    await _services.UpdateAskPageSize(AskPageSize);

                                    break;

                                case 3:
                                    Console.WriteLine("Уведіть ваш токен ");
                                    string? MonoBankToken = Console.ReadLine();

                                    if (token == null)
                                    {
                                        Console.WriteLine("Помилка token не завантажено з бази данних ");
                                        continue;
                                    }

                                    token.Token = MonoBankToken;

                                    await _services.UpdatemMonobankToken(token);
                                    token =  await _services.GetMonobankToken();

                                    break;

                                case 4:
                                    break;

                                default:
                                    throw new Exception("Не відома операція");

                            
                            }

                            break;


                        case 6:
                            return;


                        default:
                            throw new Exception("Не правильно вказані данні ");
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }
        }


        public void DisplayTransactions(List<TransactionsReadDto> transactions)
        {
            if (!transactions.Any())
            {
                Console.WriteLine("Немає транзакцiй");
            }
            else
            {
                foreach (var t in transactions)
                {
                    var tipe = t.Ammount > 0 ? "Дохiд" : "Витрата";

                    Console.WriteLine($" Дата: {t.Time:yyyy:mm:dd:mm}| Тип: {tipe}| Сумма: {Math.Abs(t.Ammount):F2} грн| Опис: {t.Descriptions}| Iмя: {t.CurentName}| Баланс: {t.Balance}");
                }
            }
        }


        public int ReadNumber()
        {

            while (true)
            {

                string? prompt = Console.ReadLine();

                if (int.TryParse(prompt, out int number))
                    return number;

                else
                {
                    Console.WriteLine("Уведіть чісло а не строку: ");
                    continue;
                }
            }
            
        }
    }
}
