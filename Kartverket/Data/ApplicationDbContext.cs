using Microsoft.EntityFrameworkCore;
using Kartverket.Models;
using System.Reflection.Emit;

namespace Kartverket.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DataSeeder _dataSeeder;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _dataSeeder = new DataSeeder();
        }

        public DbSet<Case> Case { get; set; }
        public DbSet<CaseWorker> CaseWorkers { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FylkesInfo> FylkesInfo { get; set; }
        public DbSet<KommuneInfo> KommuneInfo { get; set; }
        public DbSet<KartverketEmployee> KartverketEmployee { get; set; }
        public DbSet<CaseWorkerAssignment> CaseWorkerAssignment { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            base.OnModelCreating(modelBuilder);

            //explains that the FK isnt required, EFCore needs more than just ? in the class
            modelBuilder.Entity<CaseWorker>()
                .HasOne(cw => cw.User)
                .WithOne(u => u.CaseWorker)
                .HasForeignKey<User>(u => u.CaseWorkerUser)
                .IsRequired(false);


            //which entities are loaded into dataseeder
            modelBuilder.Entity<KommuneInfo>()
                .HasKey(k => k.KommuneInfoID);

            modelBuilder.Entity<FylkesInfo>()
                .HasKey(f => f.FylkesNameID);

            modelBuilder.Entity<Issue>()
                .HasKey(f => f.issueNo);

            modelBuilder.Entity<Status>()
                .HasKey(f => f.StatusNo);

            modelBuilder.Entity<KartverketEmployee>()
                .HasKey(f => f.EmployeeID);

            modelBuilder.Entity<User>()
                .HasKey(f => f.UserID);

            modelBuilder.Entity<CaseWorker>()
                .HasKey(f => f.CaseWorkerID);
            _dataSeeder.SeedData(modelBuilder);

        }

    }
}
