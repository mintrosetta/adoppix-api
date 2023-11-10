namespace AdopPixAPI.DTOs
{
    public class PostDto
    {
        public string PostId { get; set; }
        public List<string> ImagesNames { get; set; }
        public int LikeCount { get; set; }
        public string UserProfileImageName { get; set; }
        public string Username { get; set; }
        public int FollowerCount { get; set; }
        public string Description { get; set; }
        public List<PostCommentDto> Comments { get; set; }
        public List<PostRecommendDto> Recommends { get; set; }
        public List<string> Tags { get; set; }
    }
}
