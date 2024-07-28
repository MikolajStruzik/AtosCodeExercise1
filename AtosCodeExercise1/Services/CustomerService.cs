using AtosCodeExercise1.Data;
using AtosCodeExercise1.Interfaces;
using Microsoft.EntityFrameworkCore;
using AtosCodeExercise1.Interfaces;

namespace AtosCodeExercise1.Services
{
    public class CustomerService : ICustomerService
    {
        public CustomerDbContext _dbContext;

        public CustomerService(CustomerDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public bool CustomerExists(long id)
        {
            return _dbContext.Customers.Any(e => e.Id == id);
        }

    }


}
