using Microsoft.EntityFrameworkCore;
using Kartverket.Models;

namespace Kartverket.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Case> Case { get; set; }
        public DbSet<CaseWorker> CaseWorkers { get; set; }
        public DbSet<CaseWorkerList> CaseWorkerLists { get; set; }
        public DbSet<CaseWorkerOverview> CaseWorkerOverviews { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<KommuneInfo> KommuneInfo { get; set; }
        public DbSet<FylkesInfo> FylkesInfo { get; set; }


    }
}
