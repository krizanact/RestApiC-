using System.ComponentModel.DataAnnotations;

namespace Project.Model.Model.User
{
    public class LoginInput
    {
        [Required(ErrorMessage = "Username is required")]
        [EmailAddress(ErrorMessage = "Invalid username(email) format")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
