using Microsoft.Extensions.DependencyInjection;
using Parsing.Interface;

namespace Parsing.BackgroundServices
{
    public class BackgroundServices
    {
        private readonly TimeSpan _updateInterval;
        private readonly IServiceProvider _servicesProvider;
        private bool I_Running = true;


        public BackgroundServices(TimeSpan updateInterval, IServiceProvider serviceProvider)
        {
            _updateInterval = updateInterval;
            _servicesProvider = serviceProvider;
        }

        public void Run()
        {
            I_Running = true;
            Task.Run(async () => await RunAsync());
        }

        public void Stop()
        {
            I_Running = false;
        }


        public async Task RunAsync()
        {

            while (I_Running)
            {
                try
                {
                    using (var scope = _servicesProvider.CreateScope())
                    {
                        var transactionsServices = scope.ServiceProvider.GetRequiredService<IServices>();
                            

                        var token = await transactionsServices.GetMonobankToken();

                        if (!string.IsNullOrWhiteSpace(token.Token) && await transactionsServices.CanUpdateToken())
                        {
                            await transactionsServices.FindAndSaveTransactions(token.Token);
                            Console.WriteLine("Фонове оновлення пройшло успішно ");
                        }

                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Помилка оновення данних {ex.Message}");
                }

                await Task.Delay(_updateInterval);
            }
        }
    }
}
