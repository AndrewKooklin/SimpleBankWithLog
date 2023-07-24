using SimpleBank.Model;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SimpleBank.Data
{
    /// <summary>
    /// Установка контекста базы данных
    /// </summary>
    public class SimpleBankContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public DbSet<SalaryAccount> SalaryAccounts { get; set; }

        public DbSet<DepositAccount> DepositAccounts { get; set; }

        public SimpleBankContext() : base("SimpleBankConnectionSQLite")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqLiteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<SimpleBankContext>(modelBuilder);

            Database.SetInitializer(sqLiteConnectionInitializer);
        }
    }
}
