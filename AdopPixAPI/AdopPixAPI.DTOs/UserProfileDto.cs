namespace AdopPixAPI.DTOs
{
    public class UserProfileDto
    {
        public string CoverImage { get; set; }
        public string ProfileImage { get; set; }
        public string Username { get; set; }
        public int FollowerAmount { get; set; }
        public string Description { get; set; }
    }
}
