using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SimpleBank.Data
{
    public class MyDbContextInitializer : SqliteDropCreateDatabaseAlways<SimpleBankContext>
    {
        public MyDbContextInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
            //Database.SetInitializer<SimpleBankContext>(null);
        }

        protected override void Seed(SimpleBankContext context)
        {

        }
    }
}
