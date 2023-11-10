using AdopPixAPI.DTOs;
using AdopPixAPI.Hubs;
using AdopPixAPI.Services.UnitOfWork;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AdopPixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuctionController : ControllerBase
    {
        private readonly IServiceUnitOfWork serviceUnitOfWork;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IHubContext<AuctionBidHub> auctionBidHub;

        public AuctionController(IServiceUnitOfWork serviceUnitOfWork, 
                                 IBackgroundJobClient backgroundJobClient,
                                 IHubContext<AuctionBidHub> auctionBidHub)
        {
            this.auctionBidHub = auctionBidHub;
            this.serviceUnitOfWork = serviceUnitOfWork;
            this.backgroundJobClient = backgroundJobClient;
        }

        [HttpPost()]
        public async Task<ActionResult<ResponseDto<string>>> Create([FromForm] AuctionCreateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                string auctionId = await serviceUnitOfWork.AuctionService.CreateAsync(dto, userId);
                return Ok(new ResponseDto<string>()
                {
                    Successful = true,
                    Message = "Successful",
                    Data = auctionId
                });
            }
            return Unauthorized();
        }
        
        [HttpGet()]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll(int take, int page, bool onlyActive)
        {
            var auctions = await serviceUnitOfWork.AuctionService.GetAllAsync(take, page, onlyActive);

            List<AuctionDtoV2> auctionsDto = null;

            if (auctions.Count > 0) auctionsDto = new List<AuctionDtoV2>();

            foreach(var auction in auctions)
            {
                var ownerProfile = await serviceUnitOfWork.UserService.FindProfileById(auction.UserId);

                AuctionDtoV2 auctionDtoV2 = new AuctionDtoV2();
                auctionDtoV2.AuctionId = auction.Id;
                auctionDtoV2.Title = auction.Title;
                auctionDtoV2.Description = auction.Description;
                auctionDtoV2.StopTime = auction.Stop;
                auctionDtoV2.OwnerUsername = ownerProfile.Username;
                auctionDtoV2.OwnerProfileImage = ownerProfile.ProfileImage;

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

        [HttpGet("{auctionId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<AuctionDto>>> GetAuctionById(string auctionId)
        {
            if (auctionId != null)
            {
                var auction = await serviceUnitOfWork.AuctionService.FindByIdAsync(auctionId);
                
                if (auction != null)
                {
                    auction.Tags = await serviceUnitOfWork.AuctionService.FindTagById(auctionId);
                    return Ok(new ResponseDto<AuctionDto>()
                    {
                        Successful = true,
                        Message = "successful",
                        Data = auction
                    });
                }
                return Ok(new ResponseDto<AuctionDto>()
                {
                    Successful = false,
                    Message = "no content",
                    Data = null
                });
            }
            return BadRequest();
        }

        [HttpPost("{auctionId}/bid")]
        public async Task<ActionResult<ResponseDto<bool>>> CreateBid(string auctionId, decimal amount)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await serviceUnitOfWork.UserService.FindProfileById(userId);

            if (user == null) return Unauthorized(); // user not found
          
            if (auctionId == string.Empty || amount <= 0) return BadRequest(); // auctionId empty or amount less than 0

            var auction = await serviceUnitOfWork.AuctionService.FindByIdAsync(auctionId);
            if (auction == null) return NotFound(); // auction not found

            var userMoney = await serviceUnitOfWork.UserService.FindMoneyById(userId);
            var maxAmount = await serviceUnitOfWork.AuctionService.GetMaxBidByAuctionIdAsync(auctionId);

            if (maxAmount == null) // no user bid this auction
            {
                if (amount <= auction.OpenPrice || userMoney < auction.OpenPrice) return BadRequest(new ResponseDto<bool>()
                {
                        Successful = false,
                        Message = "You money to low.",
                        Data = false
                });
                DateTime start = DateTime.Now;
                DateTime stop = DateTime.Now.AddHours(auction.Hour);

                await serviceUnitOfWork.AuctionService.CreateBidAsync(auctionId, amount, userId);
                await serviceUnitOfWork.AuctionService.CreateEndAuctionTask(auctionId, auction.Hour);
                await serviceUnitOfWork.AuctionService.InitialTime(auctionId, start, stop);
            }
            else 
            {
                decimal minimumBid = Convert.ToDecimal(auction.CurrentBid) + auction.MinimumBid;
                bool userMoneyCondition = userMoney > maxAmount.Amount && userMoney > minimumBid;
                bool amountCondition = amount > maxAmount.Amount;
                bool minimumBidCondition = amount >= minimumBid;

                if (!userMoneyCondition || !amountCondition || !minimumBidCondition) return BadRequest(new ResponseDto<bool>()
                {
                    Successful = false,
                    Message = "You money to low.",
                    Data = false
                });

                if (amount >= auction.ClosePrice)
                {
                    await serviceUnitOfWork.AuctionService.Won(userId, auctionId, amount);

                    var userLosers = await serviceUnitOfWork.AuctionService.GetLoser(auctionId, userId);
                    if(userLosers.Count > 0)
                    {
                        foreach(var userLoser in userLosers)
                        {
                            // tracsaction money to loser auction user
                            await serviceUnitOfWork.UserService.Transaction(userLoser.UserId, userLoser.Amount);

                            // send notification to loser user
                            // notification
                        }
                    }
                    
                    // transaction money to auction owner
                    var auctionUser = await serviceUnitOfWork.UserService.FindByUsername(auction.Username);
                    await serviceUnitOfWork.UserService.Transaction(auctionUser.Id, amount);
                }
                else
                {
                    await serviceUnitOfWork.AuctionService.CreateBidAsync(auctionId, amount, userId);
                }
            }
            await auctionBidHub.Clients.All.SendAsync(auctionId, new {amount = amount,
                                                                      username = user.Username, 
                                                                      profileImage = user.ProfileImage, 
                                                                      created = DateTime.Now});
            return Ok(new ResponseDto<bool>()
            {
                Successful = true,
                Message = "Successful",
                Data = true
            });
        }

        [HttpGet("TestGetLoser")]
        public async Task<ActionResult> GetLoser(string auctionId, string userIdWon)
        {
            var result = await serviceUnitOfWork.AuctionService.GetLoser(auctionId, userIdWon);
            return Ok(result);
        }
    
        [HttpGet("TestService")]
        [AllowAnonymous]
        public async Task<ActionResult> TestService(string auctionId)
        {
            await auctionBidHub.Clients.All.SendAsync(auctionId, new {amount = 1000,
                                                                      username = "mint.colorfuls", 
                                                                      profileImage = "mint.colorfuls", 
                                                                      created = DateTime.Now});
            return Ok();
        }
    }
}
