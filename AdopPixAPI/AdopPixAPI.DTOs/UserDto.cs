using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdopPixAPI.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string UserProfileImage { get; set; }
        public string UserProileCover { get; set; }
        public string UserDescription { get; set; }
    }
}