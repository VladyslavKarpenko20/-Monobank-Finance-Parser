using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Parsing.Data;
using Parsing.Endpoints;
using Parsing.Interface;
using Parsing.Repository;
using Parsing.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Parsing.BackgroundServices;


Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.InputEncoding = System.Text.Encoding.UTF8;


var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


var services = new ServiceCollection();

services.AddScoped<IRepository, Repository>();
services.AddScoped<IServices, Services>();
services.AddScoped<Endpoint>();

services.AddDbContext<AddDbContext>(options => options.UseNpgsql(configuration["Database:ConnectionString"], optionsSql => optionsSql.EnableRetryOnFailure()));

var servicesProvider = services.BuildServiceProvider();



using (var scope = servicesProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AddDbContext>();
    await dbContext.Database.MigrateAsync();
}


var time = TimeSpan.FromHours(6);

var backgroundUpdates = new BackgroundServices(time, servicesProvider);


var endpoints = servicesProvider.GetRequiredService<Endpoint>();

backgroundUpdates.Run();

await endpoints.RunAsync();

backgroundUpdates.Stop();








