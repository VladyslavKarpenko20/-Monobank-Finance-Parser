using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parsing.Models;

namespace Parsing.Data
{
    public class AddDbContext : DbContext
    {
        public DbSet<Transactions> Transactions { get; set; }

        public DbSet<AppSettings> AppSettings { get; set; }

        public DbSet<LimitRequest> LimitRequst { get; set; }

        public DbSet<MonobankToken> Tokens {  get; set; }
        
        public AddDbContext(DbContextOptions<AddDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var settinds = new AppSettings
            {
                Id = 1 ,
                PageSize = 20,
                AskPageSize = false
            };

            var limitTime = new LimitRequest
            {
                Id = 1,
                Time = null
            };

            var token = new MonobankToken
            {
                Id = 1,
                Token = null
            };


            modelBuilder.Entity<AppSettings>()
                .HasData(settinds);

            modelBuilder.Entity<LimitRequest>()
                .HasData(limitTime);

            modelBuilder.Entity<MonobankToken>()
                .HasData(token);
        }
    }
}
