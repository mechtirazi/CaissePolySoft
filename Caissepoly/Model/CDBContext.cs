using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaissePoly.Model
{
    class CDBContext : DbContext
    {
        public CDBContext(DbContextOptions<CDBContext> options) : base(options) { }
        public CDBContext() { }
        public DbSet<Article> Article { get; set; }
        public DbSet<Famille> Famille { get; set; }
        public DbSet<Utilisateur> Utilisateur { get; set; }
        public DbSet<Vente> Vente { get; set; }

        public DbSet<Ticket> Tickets { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=caisse;Trusted_Connection=True;");
        }

        public class CDBContextFactory : IDesignTimeDbContextFactory<CDBContext>
        {
            public CDBContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<CDBContext>();
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=caisse;Trusted_Connection=True;");
                return new CDBContext(optionsBuilder.Options);
            }
        }
    }
}