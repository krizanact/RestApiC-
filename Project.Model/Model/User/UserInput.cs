using System.ComponentModel.DataAnnotations;

namespace Project.Model.Model.User
{
    public class UserInput
    {
        [Required(ErrorMessage = "Username is required")]
        [EmailAddress(ErrorMessage = "Invalid username(email) format")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(5, ErrorMessage = "At least 5 character is required")]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "Role is required")]
        [Range(1, 2, ErrorMessage = "Select role value to 1 for admin role, or 2 for default user role")]
        public int RoleId { get; set; }
    }
}
