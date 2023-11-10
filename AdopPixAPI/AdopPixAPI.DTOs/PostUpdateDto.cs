using Microsoft.AspNetCore.Http;

namespace AdopPixAPI.DTOs
{
    public class PostUpdateDto
    {
        public string PostId { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> Tags { get; set; }
    }
}
