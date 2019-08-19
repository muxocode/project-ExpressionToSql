/*using ExpresionToSql.util.test.clases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExpressionToSql.sql.test.sqlServer.context
{
    public class TestDbContext:DbContext
    {

        public DbSet<Paciente> Pacientes { get; set; }

        public string ConnectionString => "Filename=TestDatabase.db";

        public void EnsureExists()
        {
            using (var dbContext = new TestDbContext())
            {
                //Ensure database is created
                dbContext.Database.EnsureCreated();
            }
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });


            base.OnConfiguring(optionsBuilder);
        }
    }
}
*/