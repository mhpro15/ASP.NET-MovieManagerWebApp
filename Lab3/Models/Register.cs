using System.ComponentModel.DataAnnotations;

namespace Lab3.Models
{
    public class Register
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password is not the same.")]
        public string ConfirmPassword { get; set; }


    }
}
