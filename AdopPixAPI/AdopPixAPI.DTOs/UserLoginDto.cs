using System.ComponentModel.DataAnnotations;

namespace AdopPixAPI.DTOs
{
    public class UserLoginDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
