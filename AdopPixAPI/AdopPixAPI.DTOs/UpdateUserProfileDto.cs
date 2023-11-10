using Microsoft.AspNetCore.Http;

namespace AdopPixAPI.DTOs
{
    public class UpdateUserProfileDto
    {
        public string Username { get; set; }
        public IFormFile CoverImage { get; set; }
        public IFormFile ProfileImage { get; set; }
        public string Description { get; set; }
    }
}
