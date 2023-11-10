using System.ComponentModel.DataAnnotations;

namespace AdopPixAPI.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; }
        [Required]
        public bool IsAcceptArgreement { get; set; }
    }
}
