using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdopPixAPI.DTOs
{
    public class PostCreateDto
    {
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> Tags { get; set; }
    }
}
