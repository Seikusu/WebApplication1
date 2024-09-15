using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebApplication1.Models
{
    public class PeopleDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Experience> Experiences { get; set; }

        public DbSet<Users> Users { get; set; }
        public PeopleDbContext() : base("name=PeopleDbContext")
        {
            // Disable the automatic migration.
            Database.SetInitializer<PeopleDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Experiences)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.PersonID)
                .WillCascadeOnDelete(true); // Enable cascade delete

            base.OnModelCreating(modelBuilder);
        }
    }
}