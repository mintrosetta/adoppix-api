using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DTOs;
using AdopPixAPI.Services.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AdopPixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IServiceUnitOfWork serviceUnitOfWork;

        public AccountController(IServiceUnitOfWork serviceUnitOfWork)
        {
            this.serviceUnitOfWork = serviceUnitOfWork;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> Find()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();
            var userProfile = await serviceUnitOfWork.UserService.FindProfileById(userId);

            return Ok(new ResponseDto<UserProfileDto>()
            {
                Successful = true,
                Message = "Successful",
                Data = userProfile
            });
        }

        [HttpGet("{username}/profile")]
        public async Task<ActionResult> FindByUsername(string username)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();

            var userProfile = await serviceUnitOfWork.UserService.FindProfileByUsername(username);
            if (userProfile == null) return NotFound();

            return Ok(new ResponseDto<UserProfileDto>() {
                Successful = true,
                Message = "Success",
                Data = userProfile
            });
        }

        [HttpPatch()]
        public async Task<ActionResult> UpdateProfile([FromForm] UpdateUserProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await serviceUnitOfWork.UserService.UpdateProfileByUserId(dto, userId);
                return Ok(new ResponseDto<bool>()
                {
                    Successful = true,
                    Message = "Successful",
                    Data = true
                });
            }
            return Unauthorized();
        }
    
        [HttpGet("{username}/follow")]
        public async Task<ActionResult> IsFollow(string username)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized();

            var userProfile = await serviceUnitOfWork.UserService.FindByUsername(username);
            if(userProfile == null) return NotFound(new ResponseDto<bool>() {
                Successful = false,
                Message = "Username not found",
                Data = false
            });

            var isFollow = await serviceUnitOfWork.UserService.IsFollow(userId, userProfile.Id);
            return Ok(new ResponseDto<bool>() {
                Successful = true,
                Message = "Username not found",
                Data = (isFollow != null)
            });
        }
        [HttpPost("{username}/follow")]
        public async Task<ActionResult> Follow(string username)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(username == null) return Unauthorized();

            var user = await serviceUnitOfWork.UserService.FindByUsername(username);
            if(user == null) return NotFound();

            bool following = await serviceUnitOfWork.UserService.Follow(currentUserId, user.Id);

            return Ok(new ResponseDto<bool>()
                {
                    Successful = true,
                    Message = "Successful",
                    Data = following
                });
        }
    
        [HttpGet("{username}/post")]
        public async Task<ActionResult> FindUserPostByUsername(string username, int take, int page)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await serviceUnitOfWork.UserService.FindByUsername(username);
            if (user == null) return NotFound(new ResponseDto<List<PostDto>>() {
                Successful = false,
                Message = "Content not found",
                Data = null
            });

            var posts = await serviceUnitOfWork.PostService.FindPostByUserId(user.Id, take, page);

            List<PostDto> postDtos = new List<PostDto>();
            foreach(var post in posts)
            {
                PostDto postDto = new PostDto();
                postDto.PostId = post.Id;

                var images = await serviceUnitOfWork.PostService.FindImageByPostIdAsync(post.Id);
                postDto.ImagesNames = images.Select(x => x.Name).ToList();

                var rawTags = await serviceUnitOfWork.PostService.FindTagByPostIdAsync(post.Id);

                List<string> tagNames = new List<string>();
                foreach(var rawTag in rawTags)
                {
                    var tagName = await serviceUnitOfWork.TagService.FindByIdAsync(rawTag.TagId);
                    tagNames.Add(tagName);
                }
                postDto.Tags = tagNames;
                postDtos.Add(postDto);
            }

            return Ok(new ResponseDto<List<PostDto>>() {
                Successful = true,
                Message = "Succesful",
                Data = postDtos
            });
        }
    
        [HttpGet("{username}/auction")]
        [AllowAnonymous]
        public async Task<ActionResult> FindUserAuctionByUsername(string username, int take, int page, bool onlyActive)
        {
            var user = await serviceUnitOfWork.UserService.FindByUsername(username);
            if (user == null) return NotFound(new ResponseDto<List<PostDto>>() {
                Successful = false,
                Message = "Content not found",
                Data = null
            });

            List<AuctionDtoV2> auctionsDto = null;

            var auctions = await serviceUnitOfWork.AuctionService.FindByUserIdAsync(user.Id, take, page, onlyActive);

            if (auctions.Count > 0) auctionsDto = new List<AuctionDtoV2>();

            foreach(var auction in auctions)
            {
                AuctionDtoV2 auctionDtoV2 = new AuctionDtoV2();
                auctionDtoV2.AuctionId = auction.Id;
                auctionDtoV2.Title = auction.Title;
                auctionDtoV2.Description = auction.Description;
                auctionDtoV2.StopTime = auction.Stop;

                var lastBid = await serviceUnitOfWork.AuctionService.GetMaxBidByAuctionIdAsync(auction.Id);
                if(lastBid != null)
                {
                    var lastUserBid = await serviceUnitOfWork.UserService.FindProfileById(lastBid.UserId);
                    auctionDtoV2.CurrentBid = lastBid.Amount;
                    auctionDtoV2.UsernameCurrentBid = lastUserBid.Username;
                }
                else 
                {
                    auctionDtoV2.CurrentBid = 0;
                    auctionDtoV2.UsernameCurrentBid = string.Empty;
                }

                auctionDtoV2.MinimumBid = auction.MinimumBid;
                auctionDtoV2.OpenPrice = auction.OpenPrice;
                auctionDtoV2.ClosePrice = auction.ClosePrice;
                auctionDtoV2.Hour = auction.Hour;
                auctionDtoV2.Images = await serviceUnitOfWork.AuctionService.FindImageByAuctionId(auction.Id);

                auctionsDto.Add(auctionDtoV2);
            }
            
            return Ok(new ResponseDto<List<AuctionDtoV2>>() {
                Successful = true,
                Message = "Successful",
                Data = auctionsDto
            });
        }
    
        [HttpGet("Money")]
        public async Task<ActionResult> FindMoneyByUsername()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            decimal userMoney = await serviceUnitOfWork.UserService.FindMoneyById(userId);
            return Ok(new ResponseDto<decimal>() {
                Successful = true,
                Message = "Successful",
                Data = userMoney,
            });
        }

        [HttpGet("users")]
        public async Task<ActionResult> SearchUsers(string search)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var users = await serviceUnitOfWork.UserService.Search(search);         

            return Ok(new ResponseDto<List<string>>() {
                Successful = true,
                Message = "Successful",
                Data = users,
            });
        }
    }
}
