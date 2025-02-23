using Microsoft.EntityFrameworkCore;
using RegularExpressionTask3.Models;

namespace RegularExpressionTask3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    }
}
