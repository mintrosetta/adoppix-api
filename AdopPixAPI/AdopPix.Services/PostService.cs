using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.UnitOfWork;
using AdopPixAPI.DTOs;
using AdopPixAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.Services
{
    public class PostService : IPostService
    {
        private readonly IEntityUnitOfWork entityUnitOfWork;
        private readonly IUploadFileService uploadFileService;

        public PostService(IEntityUnitOfWork entityUnitOfWork, IUploadFileService uploadFileService)
        {
            this.entityUnitOfWork = entityUnitOfWork;
            this.uploadFileService = uploadFileService;
        }

        public async Task CreateAsync(PostCreateDto postCreateDto, string userId)
        {
            if(postCreateDto != null)
            {
                string postId = Guid.NewGuid().ToString();
                DateTime created = DateTime.Now;
                Post post = new Post()
                {
                    Id = postId,
                    UserId = userId,
                    Description = postCreateDto.Description,
                    Created = created,
                    IsActive = true,
                };
                await entityUnitOfWork.PostRepository.Add(post);

                List<PostImage> images = new List<PostImage>();
                foreach(IFormFile image in postCreateDto.Images)
                {
                    var ext = Path.GetExtension(image.FileName);
                    string imageName = Guid.NewGuid().ToString() + ext;
                    await uploadFileService.Upload(image, imageName);

                    PostImage postImage = new PostImage()
                    {
                        PostId=postId,
                        Name = imageName,
                        Created = created,
                        IsActive=true,
                    };
                    images.Add(postImage);
                }
                await entityUnitOfWork.PostImageRepository.AddRange(images);

                List<PostTag> postTags = new List<PostTag>();
                foreach (string tagName in postCreateDto.Tags)
                {
                    var oldTag = await entityUnitOfWork.TagRepository.Find(t => t.Title == tagName).FirstOrDefaultAsync();
                    string tagId;
                    if (oldTag == null)
                    {
                        tagId = Guid.NewGuid().ToString();
                        Tag tag = new Tag()
                        {
                            Id = tagId,
                            Title = tagName,
                            Created = DateTime.Now,
                            IsActive = true,
                        };
                        await entityUnitOfWork.TagRepository.Add(tag);
                    }
                    else
                    {
                        tagId = oldTag.Id;
                    }

                    PostTag postTag = new PostTag()
                    {
                        PostId = postId,
                        TagId = tagId,
                        Created = created
                    };
                    postTags.Add(postTag);
                }
                await entityUnitOfWork.PostTagRepository.AddRange(postTags);

                await entityUnitOfWork.CompleteAsync();
            }
        }

        public async Task<PostDto> FindByIdAsync(string postId, string userId)
        {
            if(postId != null)
            {
                var post = await entityUnitOfWork.PostRepository.FindById(postId);
                var user = await entityUnitOfWork.UserProfileRepository.Find(x => x.UserId == post.UserId)
                                                                       .FirstOrDefaultAsync();

                var followerCount = await entityUnitOfWork.UserFollowRepository.Find(f => f.ToUserId == user.UserId)
                                                                               .CountAsync();

                if(post != null && user != null)
                {
                    var imageNames = await entityUnitOfWork.PostImageRepository.Find(p => p.PostId == post.Id)
                                                                               .Select(i => i.Name)
                                                                               .ToListAsync();

                    var likeCount = await entityUnitOfWork.PostLikeRepository.Find(l => l.PostId == post.Id)
                                                                             .CountAsync();

                    var rawComments = await entityUnitOfWork.PostCommentRepository.Find(c => c.PostId == post.Id)
                                                                                  .ToListAsync();
                    List<PostCommentDto> comments = new List<PostCommentDto>();
                    foreach(var comment in rawComments)
                    {
                        var userComment = await entityUnitOfWork.UserProfileRepository.Find(x => x.UserId == comment.UserId)
                                                                               .FirstOrDefaultAsync();
                        PostCommentDto postCommentDto = new PostCommentDto()
                        {
                            ProfileImageName = userComment.ProfileImage,
                            Username = userComment.Username,
                            Description = comment.Description
                        };
                        comments.Add(postCommentDto);
                    }

                    var rawPostTags = await entityUnitOfWork.PostTagRepository.Find(r => r.PostId == post.Id)
                                                                              .ToListAsync();
                    List<string> postTags = new List<string>();
                    foreach (var postTag in rawPostTags)
                    {
                        var tag = await entityUnitOfWork.TagRepository.FindById(postTag.TagId);
                        postTags.Add(tag.Title);
                    }

                    PostDto postDto = new PostDto()
                    {
                        ImagesNames = imageNames,
                        LikeCount = likeCount,

                        UserProfileImageName = user.ProfileImage,
                        Username = user.Username,
                        FollowerCount = followerCount,
                        Description = post.Description,
                        Comments = comments,
                        Recommends = null,
                        Tags = postTags

                    };
                    return postDto;
                }
            }

            return null;
        }

        public async Task<Post> FindByIdAsync(string postId)
        {
            return await entityUnitOfWork.PostRepository.FindById(postId);
        }

        public async Task<List<PostImage>> FindImageByPostIdAsync(string postId)
        {
            return await entityUnitOfWork.PostImageRepository.Find(f => f.PostId == postId)
                                                             .ToListAsync();
        }

        public async Task<List<PostTag>> FindTagByPostIdAsync(string postId)
        {
            return await entityUnitOfWork.PostTagRepository.Find(t => t.PostId == postId)
                                                           .ToListAsync();
        }

        public async Task<PostLike> FindLikeAsync(string postId, string userId)
        {
            return await entityUnitOfWork.PostLikeRepository.Find(l => l.PostId == postId && l.UserId == userId)
                                                            .FirstOrDefaultAsync();
        }

        public async Task LikeToggleAsync(string postId, string userId)
        {
            var postLike = await entityUnitOfWork.PostLikeRepository.Find(l => l.PostId == postId && l.UserId == userId)
                                                                    .FirstOrDefaultAsync();

            if(postLike == null)
            {
                var like = new PostLike()
                {
                    UserId = userId,
                    PostId = postId,
                    Created = DateTime.Now
                };

                await entityUnitOfWork.PostLikeRepository.Add(like);   
            }
            else
            {
                entityUnitOfWork.PostLikeRepository.Remove(postLike);
            }

            await entityUnitOfWork.CompleteAsync();
        }
    
        public async Task UpdateAsync(PostUpdateDto postUpdateDto, 
                                      Post post, 
                                      List<PostImage> postImages, 
                                      List<PostTag> postTags)
        {
            // edit description
            if (postUpdateDto.Description != post.Description) post.Description = postUpdateDto.Description;
            entityUnitOfWork.PostRepository.Update(post);

            // edit image
            List<PostImage> newPostImages = new List<PostImage>();
            foreach (var newImg in postUpdateDto.Images)
            {
                var img = postImages.FirstOrDefault(x => x.Name == newImg.FileName);
                if (img != null)
                {
                    newPostImages.Add(img);
                }
                else
                {
                    var ext = Path.GetExtension(newImg.FileName);
                    string imageName = Guid.NewGuid().ToString() + ext;
                    await uploadFileService.Upload(newImg, imageName);

                    PostImage postImage = new PostImage()
                    {
                        PostId = post.Id,
                        Name = imageName,
                        Created = DateTime.Now,
                        IsActive = true,
                    };

                    newPostImages.Add(postImage);
                }
            }

            foreach(var currentImg in postImages)
            {
                var img = newPostImages.FirstOrDefault(x => x.Name == currentImg.Name);
                if(img != null)
                {
                    postImages.Remove(img);
                }
            }

            if(postImages.Count > 0) 
            {
                entityUnitOfWork.PostImageRepository.RemoveRange(postImages);
            }
            await entityUnitOfWork.PostImageRepository.AddRange(newPostImages);

            // edit tag
            List<PostTag> newPostTags = new List<PostTag>();
            foreach(var newTag in postUpdateDto.Tags)
            {
                var tagId = await entityUnitOfWork.TagRepository.Find(x => x.Title == newTag).Select(x => x.Id).FirstOrDefaultAsync();
                if (tagId != null)
                {
                    var currentPostTag = await entityUnitOfWork.PostTagRepository.Find(x => x.PostId == post.Id && x.TagId == tagId).FirstOrDefaultAsync();
                    if (currentPostTag != null) newPostTags.Add(currentPostTag);
                }
                else
                {
                    tagId = Guid.NewGuid().ToString();
                    Tag tag = new Tag()
                    {
                        Id = tagId,
                        Title = newTag,
                        Created = DateTime.Now,
                        IsActive = true,
                    };
                    await entityUnitOfWork.TagRepository.Add(tag);

                    PostTag postTag = new PostTag()
                    {
                        PostId = post.Id,
                        TagId = tagId,
                        Created = DateTime.Now
                    };
                    newPostTags.Add(postTag);
                }
            }
            await entityUnitOfWork.PostTagRepository.AddRange(newPostTags);
            
            await entityUnitOfWork.CompleteAsync();
        }
    
        public async Task CreateComment(string userId , string postId, CreateCommentDto createCommentDto)
        {
            PostComment postComment = new PostComment()
            {
                PostId = postId,
                UserId = userId,
                Description = createCommentDto.Comment,
                Created = DateTime.Now,
                IsActive = true
            };
            await entityUnitOfWork.PostCommentRepository.Add(postComment);

            await entityUnitOfWork.CompleteAsync();
        }
    
        public async Task RemoveAsync(string postId)
        {
            var post = await entityUnitOfWork.PostRepository.Find(x => x.Id == postId).FirstOrDefaultAsync();
            post.IsActive = false;
            entityUnitOfWork.PostRepository.Update(post);
            await entityUnitOfWork.CompleteAsync();
        }
    
        public async Task<bool> IsRemove(string postId)
        {
            var post = await entityUnitOfWork.PostRepository.Find(x => x.Id == postId).FirstOrDefaultAsync();
            return !post.IsActive;
        }
    
        public async Task<List<Post>> FindPostByUserId(string userId, int take, int page)
        {
            var posts = await entityUnitOfWork.PostRepository.Find(x => x.UserId == userId)
                                                             .OrderByDescending(x => x.Created)
                                                             .Skip(take * page)
                                                             .Take(take)
                                                             .ToListAsync();
            return posts;
        }

        public async Task<List<Post>> GetAllAsync(int take, int page, List<string> userFollows)
        {
            var posts = entityUnitOfWork.PostRepository.Find(x => x.IsActive == true)
                                                       .Where(x => userFollows.Contains(x.UserId));

            return await posts.OrderByDescending(x => x.Created).Skip(take * page).Take(take).ToListAsync();
        }

        public async Task<bool> UserIsLike(string userId, string postId)
        {
            return await entityUnitOfWork.PostLikeRepository.Find(x => x.UserId == userId && x.PostId == postId)
                                                            .FirstOrDefaultAsync() != null;
        }

        public async Task<int> GetLikeCountByPostId(string postId)
        {
            return await entityUnitOfWork.PostLikeRepository.Find(x => x.PostId == postId).CountAsync();
        }
    }
}
