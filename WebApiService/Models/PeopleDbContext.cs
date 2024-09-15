using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApiService.Models
{
    public class PeopleDbContext : DbContext
    {
        public PeopleDbContext() : base("name=PeopleDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false; // Nonaktifkan lazy loading
            this.Configuration.ProxyCreationEnabled = false; // Nonaktifkan proxy creation
                                                             // Disable the automatic migration.
            Database.SetInitializer<PeopleDbContext>(null);
        }
        public DbSet<Person> People { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Users> Users { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Experiences)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.PersonID)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}