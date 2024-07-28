using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtosCodeExercise1.Data;
using AtosCodeExercise1.Models;
using System.Runtime.CompilerServices;
using AtosCodeExercise1.Interfaces;

namespace AtosCodeExercise1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(CustomerDbContext context, ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _context = context;
            _customerService = customerService;
            _logger = logger;
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _logger.LogInformation("Started method PostCustomer...");
            
            if (_customerService.CustomerExists(customer.Id))
            {
                _logger.LogWarning($"User with Id {customer.Id} already exists");

                return Conflict(new { message = $"Customer of  Id = {customer.Id} already exists" });
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();


            _logger.LogInformation("Customer created successfully with Id = {CustomerId}", customer.Id);
            return CreatedAtAction("PostCustomer", new { id = customer.Id }, customer);
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            _logger.LogInformation("Started GetCustomers method...");
            var customers = await _context.Customers.ToListAsync();
            if (customers == null || customers.Count == 0)
            {
                _logger.LogWarning("No customers found");
                return NoContent();
            }

            _logger.LogInformation($"Retrieved {customers.Count} customers.");
            return await _context.Customers.ToListAsync();
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            _logger.LogInformation("Started DeleteCustomer method...");

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Attempted to delete customer with Id = {CustomerId}, but it does not exist.", id);
                return NotFound( new { message = $"Customer with Id = {id} doesn't exist" });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Customer with Id = {CustomerId} deleted successfully.", id);
            return NoContent();
        }
    }
        
}
