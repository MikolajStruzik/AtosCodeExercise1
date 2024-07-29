using System.ComponentModel.DataAnnotations;

namespace AtosCodeExercise1.Models
{
    public class Customer
    {
        [Key]
        public long Id { get; set; }
        
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name can't be longer than 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name can't be longer than 100 characters")]
        public string LastName { get; set; }
    }
}
