using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DTOs;
using AdopPixAPI.Services.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdopPixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IServiceUnitOfWork serviceUnitOfWork;

        public PostController(IServiceUnitOfWork serviceUnitOfWork)
        {
            this.serviceUnitOfWork = serviceUnitOfWork;
        }

        [HttpPost()]
        public async Task<ActionResult> Create([FromForm] PostCreateDto postCreateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId != null)
            {
                var user = serviceUnitOfWork.UserService.FindById(userId);
                if(user != null)
                {
                    await serviceUnitOfWork.PostService.CreateAsync(postCreateDto, userId);
                    return Ok(new ResponseDto<string>()
                    {
                        Successful = true,
                        Message = "Successful",
                        Data = "Successful"
                    });
                }
            }
            return Unauthorized();
        }

        [HttpGet()]
        public async Task<ActionResult> GetPosts(int take, int page) 
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();

            var follows = await serviceUnitOfWork.UserService.FindFollowByUserId(userId);

            var posts = await serviceUnitOfWork.PostService.GetAllAsync(take, page, follows);
            
            List<PostDtoV2> postDtoV2s = null;
            if(posts.Count > 0) postDtoV2s = new List<PostDtoV2>();

            foreach(var post in posts)
            {
                var images = await serviceUnitOfWork.PostService.FindImageByPostIdAsync(post.Id);
                var isLike = await serviceUnitOfWork.PostService.UserIsLike(userId, post.Id);
                var userPost = await serviceUnitOfWork.UserService.FindProfileById(post.UserId);
                var likeCount = await serviceUnitOfWork.PostService.GetLikeCountByPostId(post.Id);
                
                PostDtoV2 postDtoV2 = new PostDtoV2()
                {
                    PostId = post.Id,
                    imageProfileName = userPost.ProfileImage,
                    Username = userPost.Username,
                    Created = post.Created,
                    Images = images.Select(x => x.Name).ToList(),
                    IsLike = isLike,
                    Description = post.Description,
                    LikeCount = likeCount
                };

                postDtoV2s.Add(postDtoV2);
            }

            return Ok(new ResponseDto<List<PostDtoV2>>() {
                Successful = true,
                Message = "Successful",
                Data = postDtoV2s
            });
        }

        [HttpGet("{postId}")]
        public async Task<ActionResult<ResponseDto<PostDto>>> FindById(string postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();

            var user =  await serviceUnitOfWork.UserService.FindById(userId);
            if(user == null) return Unauthorized();

            var post = await serviceUnitOfWork.PostService.FindByIdAsync(postId, userId);
            if(post == null) return NotFound(new ResponseDto<PostDto>()
                                    {
                                        Successful = false,
                                        Message = "Post not found",
                                        Data = null
                                    });

            var postIsRemove = await serviceUnitOfWork.PostService.IsRemove(postId);
            if(postIsRemove) return NotFound(new ResponseDto<PostDto>()
                                    {
                                        Successful = false,
                                        Message = "The post has been deleted",
                                        Data = null
                                    });
            return Ok(new ResponseDto<PostDto>()
            {
                Successful = true,
                Message = "Post not found",
                Data = post
            });
        }

        [HttpPost("{postId}/like")]
        public async Task<ActionResult> Like(string postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var post = await serviceUnitOfWork.PostService.FindByIdAsync(postId, userId);
            if (post == null) return BadRequest();

            await serviceUnitOfWork.PostService.LikeToggleAsync(postId, userId);

            return Ok();
            
        }

        [HttpPatch()]
        public async Task<ActionResult> Update([FromForm] PostUpdateDto postUpdateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var post = await serviceUnitOfWork.PostService.FindByIdAsync(postUpdateDto.PostId);
            if (post == null) return BadRequest();

            if (post.UserId != userId) return BadRequest();

            var postImgs = await serviceUnitOfWork.PostService.FindImageByPostIdAsync(postUpdateDto.PostId);
            if(postImgs == null) return BadRequest();

            var postTags = await serviceUnitOfWork.PostService.FindTagByPostIdAsync(postUpdateDto.PostId);
            if(postTags == null) return BadRequest();

            await serviceUnitOfWork.PostService.UpdateAsync(postUpdateDto, post, postImgs, postTags);

            return Ok();
        }

        [HttpPost("{postId}/comment")]
        public async Task<ActionResult> CreateComment(string postId, [FromBody] CreateCommentDto createCommentDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();

            if(createCommentDto == null) return BadRequest();

            var post = await serviceUnitOfWork.PostService.FindByIdAsync(postId);
            if(post == null) return BadRequest();

            await serviceUnitOfWork.PostService.CreateComment(userId, postId, createCommentDto);

            return Ok(new ResponseDto<bool>() {
                Successful = true,
                Message = "Successful",
                Data = true
            });
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult> Remove(string postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();

            if(postId == string.Empty) return BadRequest();

            var post = await serviceUnitOfWork.PostService.FindByIdAsync(postId);
            if(post == null) return NotFound();

            await serviceUnitOfWork.PostService.RemoveAsync(postId);

            return Ok(new ResponseDto<bool>() {
                Successful = true,
                Message = "Successful",
                Data = true
            });
        }
    }
}
