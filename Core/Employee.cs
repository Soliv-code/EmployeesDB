using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        [Required]
        public DateOnly DataOfBirth { get; set; }
        [Required]
        public decimal Salary { get; set; }
    }
}
