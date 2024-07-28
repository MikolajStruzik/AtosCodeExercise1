using AtosCodeExercise1.Models;
using Microsoft.EntityFrameworkCore;

namespace AtosCodeExercise1.Data
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) 
            : base(options) 
        { 
        }

        public DbSet<Customer> Customers { get; set; }

    }
}
