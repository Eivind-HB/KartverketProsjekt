using Microsoft.EntityFrameworkCore;
using Kartverket.Models;

namespace Kartverket.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<GeoChange> GeoChanges { get; set; }
    }
}
