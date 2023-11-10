using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DTOs;

namespace AdopPixAPI.Services.IServices
{
    public interface IPostService
    {
        Task CreateAsync(PostCreateDto postCreateDto, string userId);

        Task<Post> FindByIdAsync(string postId);
        Task<List<Post>> FindPostByUserId(string userId, int take, int page);
        Task<List<PostImage>> FindImageByPostIdAsync(string postId);
        Task<List<PostTag>> FindTagByPostIdAsync(string postId);
        Task<List<Post>> GetAllAsync(int take, int page, List<string> userFollows);
        Task<int> GetLikeCountByPostId(string postId);


        Task<PostDto> FindByIdAsync(string postId, string userId);
        Task<PostLike> FindLikeAsync(string postId, string userId);
        Task LikeToggleAsync(string postId, string userId);
        Task UpdateAsync(PostUpdateDto postUpdateDto, 
                         Post post, 
                         List<PostImage> postImages, 
                         List<PostTag> postTags);
        Task<bool> UserIsLike(string userId, string postId);
        Task CreateComment(string userId , string postId, CreateCommentDto createCommentDto);
        Task RemoveAsync(string postId);
        Task<bool> IsRemove(string postId);
    }
}
