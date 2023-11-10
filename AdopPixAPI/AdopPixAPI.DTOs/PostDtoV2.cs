namespace AdopPixAPI.DTOs
{
    public class PostDtoV2
    {
        public string PostId { get; set; }
        public string imageProfileName { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public List<string> Images  { get; set; }
        public bool IsLike { get; set; }
        public int LikeCount { get; set; }
        public string Description { get; set; }
    }
}