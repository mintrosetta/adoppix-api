using System.Data;
using System.Data.SqlClient;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.UnitOfWork;
using AdopPixAPI.DTOs;
using AdopPixAPI.Services.IServices;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace AdopPixAPI.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IEntityUnitOfWork entityUnitOfWork;
        private readonly IUploadFileService uploadFileService;
        private readonly IBackgroundJobClient backgroundJobClient;

        public AuctionService(IEntityUnitOfWork entityUnitOfWork, 
                              IUploadFileService uploadFileService,
                              IBackgroundJobClient backgroundJobClient)
        {
            this.entityUnitOfWork = entityUnitOfWork;
            this.uploadFileService = uploadFileService;
            this.backgroundJobClient = backgroundJobClient;
        }
        public async Task<string> CreateAsync(AuctionCreateDto dto, string userId)
        {
            string auctionId = Guid.NewGuid().ToString();
            Auction auction = new Auction()
            {
                Id = auctionId,
                UserId = userId,
                Title = dto.Title,
                Hour = dto.Hour,
                Description = dto.Description,
                OpenPrice = dto.OpenPrice,
                ClosePrice = dto.ClosePrice,
                MinimumBid = dto.MinimumBit,
                Created = DateTime.Now,
                IsActive = true,
                IsWon = false
            };
            await entityUnitOfWork.AuctionRepository.Add(auction);

            foreach (string tagName in dto.Tags)
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

                AuctionTag auctionTag = new AuctionTag()
                {
                    AuctionId = auctionId,
                    TagId = tagId,
                    Created = DateTime.Now,
                };
                await entityUnitOfWork.AuctionTagRepository.Add(auctionTag);

            }

            foreach (IFormFile file in dto.Images)
            {
                string fileName = Guid.NewGuid().ToString();
                await uploadFileService.Upload(file, fileName);
                var ext = Path.GetExtension(file.FileName);

                AuctionImage auctionImage = new AuctionImage()
                {
                    AuctionId = auctionId,
                    ImageTypeId = 2,
                    Name = fileName + ext,
                    Created = DateTime.Now,
                    IsActive = true
                };
                await entityUnitOfWork.AuctionImageRepository.Add(auctionImage);
            }

            await entityUnitOfWork.CompleteAsync();
            return auctionId;
        }

        public async Task CreateBidAsync(string auctionId, decimal amount, string userId)
        {
            AuctionBid auctionBid = new AuctionBid()
            {
                AuctionId = auctionId,
                Amount = amount,
                Created = DateTime.Now,
                UserId = userId
            };
            await entityUnitOfWork.AuctionBidRepository.Add(auctionBid);
            
            var user = await entityUnitOfWork.UserProfileRepository.Find(x => x.UserId == userId).FirstOrDefaultAsync();
            user.Money -= amount;
            user.Updated = DateTime.Now;

            entityUnitOfWork.UserProfileRepository.Update(user);

            await entityUnitOfWork.CompleteAsync();
        }

        public async Task<List<string>> FindTagById(string auctionId)
        {
            var tags = await entityUnitOfWork.AuctionTagRepository.Find(x => x.AuctionId == auctionId).ToListAsync();
            var tagNames = new List<string>();
            foreach(var tag in tags)
            {
                var tagName = await entityUnitOfWork.TagRepository.Find(x => x.Id == tag.TagId).Select(x => x.Title).FirstOrDefaultAsync();
                tagNames.Add(tagName);
            }
            return tagNames;
        }

        public async Task<AuctionDto> FindByIdAsync(string auctionId)
        {
            var auction = await entityUnitOfWork.AuctionRepository.FindById(auctionId);
            
            if(auction != null)
            {
                var auctionImages = await entityUnitOfWork.AuctionImageRepository.Find(a => a.AuctionId == auction.Id)
                                                                                 .Select(a => a.Name)
                                                                                 .ToListAsync();
                var auctionBids = await entityUnitOfWork.AuctionBidRepository.Find(b => b.AuctionId == auctionId)
                                                                             .ToListAsync();

                var userOwnerAuction = await entityUnitOfWork.UserProfileRepository.FindById(auction.UserId);

                AuctionBid currentBid = null;
                UserProfile currentBidUser = null;
                if (auctionBids.Count > 0)
                {
                    currentBid = auctionBids.MaxBy(x => x.Amount);
                    currentBidUser = await entityUnitOfWork.UserProfileRepository.FindById(currentBid.UserId);
                }
                                          
                AuctionDto auctionDto = new AuctionDto()
                {
                    Title = auction.Title,
                    Description = auction.Description,
                    Images = auctionImages,
                    MinimumBid = auction.MinimumBid,
                    OpenPrice = auction.OpenPrice,
                    ClosePrice = auction.ClosePrice,
                    StopTime = auction.Stop,
                    BidHistories = new List<AuctionBidHistory>(),
                    Hour = auction.Hour,
                    
                    Username = userOwnerAuction.Username,
                    UserProfileImage = userOwnerAuction.ProfileImage,
                    UserProileCover = userOwnerAuction.CoverImage,
                    UserDescription = userOwnerAuction.Description
                };

                if (currentBid != null) auctionDto.CurrentBid = currentBid.Amount;
                if (currentBidUser != null) auctionDto.UsernameCurrentBid = currentBidUser.Username;

                if(auctionBids != null)
                {
                    foreach (var auctionBid in auctionBids)
                    {
                        var bidUser = await entityUnitOfWork.UserProfileRepository.FindByUserId(auctionBid.UserId);
                        AuctionBidHistory auctionBidHistory = new AuctionBidHistory()
                        {
                            Username = bidUser.Username,
                            AmountBid = auctionBid.Amount,
                            ProfileImage = bidUser.ProfileImage,
                            Created = bidUser.Created,
                        };
                        auctionDto.BidHistories.Add(auctionBidHistory);
                    }
                }

                return auctionDto;
            }
            return null;
        }

        public async Task<List<Auction>> FindByUserIdAsync(string userId, int take, int page, bool onlyActive)
        {
            var auctions = entityUnitOfWork.AuctionRepository.Find(x => x.UserId == userId);
            if (onlyActive)
            {
                auctions =  auctions.Where(x => x.Start != null && x.Stop != null);
            }
            return await auctions.Skip(take * page).Take(take).ToListAsync();
        }

        public async Task<AuctionBid> GetMaxBidByAuctionIdAsync(string auctionId)
        {
            var maxBid = await entityUnitOfWork.AuctionBidRepository.Find(x => x.AuctionId == auctionId)
                                                                    .ToListAsync();
            return maxBid.MaxBy(x => x.Amount);
        }

        public async Task Won(string userId, string auctionId, decimal amount)
        {
            if (userId != null && auctionId != null)
            {
                AuctionWon auctionWon = new AuctionWon()
                {
                    UserId = userId,
                    AuctionId = auctionId,
                    Amount = amount,
                    Created = DateTime.Now,
                    IsActive = true
                };
                await entityUnitOfWork.AuctionWonRepository.Add(auctionWon);
                var auction = await entityUnitOfWork.AuctionRepository.Find(x => x.Id == auctionId).FirstOrDefaultAsync();
                auction.IsWon = true;
                entityUnitOfWork.AuctionRepository.Update(auction);

                await entityUnitOfWork.CompleteAsync();
            }
        }

        public async Task CreateEndAuctionTask(string auctionId, int time)
        {
            Console.WriteLine($"Start background job of auction {auctionId}");
            var jobId = backgroundJobClient.Schedule(() => CloseAuction(auctionId), TimeSpan.FromHours(time));
            var auction = await entityUnitOfWork.AuctionRepository.FindById(auctionId);
            auction.JobId = jobId;
            entityUnitOfWork.AuctionRepository.Update(auction);
            await entityUnitOfWork.CompleteAsync();
        }

        public async Task<List<UserLoserAuctionDto>> GetLoser(string auctionId, string userIdWon)
        {
            var auctionBids = await entityUnitOfWork.AuctionBidRepository.Find(x => x.AuctionId == auctionId && x.UserId != userIdWon)
                                                                         .ToListAsync();
            
            List<UserLoserAuctionDto> userLoserAuctionDtos = new List<UserLoserAuctionDto>();

            for(int index=0; index < auctionBids.Count; index++) 
            {
                var currentUserBid = userLoserAuctionDtos.Where(x => x.UserId == auctionBids[index].UserId).FirstOrDefault(); 
                if (currentUserBid != null)
                {
                    int loserIndex = userLoserAuctionDtos.FindIndex(x => x.UserId == currentUserBid.UserId);
                    userLoserAuctionDtos[loserIndex].Amount += currentUserBid.Amount;
                }
                else 
                {
                    UserLoserAuctionDto userLoserAuctionDto = new UserLoserAuctionDto()
                    {
                        UserId = currentUserBid.UserId,
                        Amount = currentUserBid.Amount
                    };
                    userLoserAuctionDtos.Add(userLoserAuctionDto);
                }
            }

            return userLoserAuctionDtos;
        } 

        public async Task InitialTime(string auctionId, DateTime start, DateTime stop)
        {
            var auction = await entityUnitOfWork.AuctionRepository.FindById(auctionId);
            auction.Start = start;
            auction.Stop = stop;
            entityUnitOfWork.AuctionRepository.Update(auction);
            await entityUnitOfWork.CompleteAsync();
        }

        public void Test()
        {
            var auction = LocalFindById("3f40c66f-bb18-4e19-8c96-371379d62b10");
            var max = GetMaxBid("3f40c66f-bb18-4e19-8c96-371379d62b10");
        }
        public static void CloseAuction(string auctionId)
        {
            Console.WriteLine(555555);
            var auction = LocalFindById(auctionId);
            if (auction != null)
            {
                var maxBid = GetMaxBid(auctionId);
                LocalWon(maxBid.UserId, auctionId, maxBid.Amount);
                Console.WriteLine($"The auction already has a winner. {maxBid.UserId}");
            }
        }

        public static Auction LocalFindById(string auctionId)
        {
            Auction auction = null;
            using(SqlConnection connection = new SqlConnection("Server=119.59.96.90;Database=adoppix_db;User ID=adoppix_backend;Password=Bto#97p84;"))
            {
                using(SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GetAuctionById";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@AuctionId", SqlDbType.NVarChar).Value = auctionId;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        auction = new Auction();
                        auction.Id = reader["auctionId"].ToString();
                        auction.UserId = reader["userId"].ToString();
                        if(!reader.IsDBNull(reader.GetOrdinal("jobId"))) auction.JobId = reader["jobId"].ToString();
                    }
                    connection.Close();
                }
            }
            return auction;
        }

        public static void LocalWon(string userId, string auctionId, decimal amount)
        {
            using(SqlConnection connection = new SqlConnection("Server=119.59.96.90;Database=adoppix_db;User ID=adoppix_backend;Password=Bto#97p84;"))
            {
                using(SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "CreateWon";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userId;
                    command.Parameters.Add("@AuctionId", SqlDbType.NVarChar).Value = auctionId;
                    command.Parameters.Add("@Amount", SqlDbType.NVarChar).Value = amount;
                    command.Parameters.Add("@Create", SqlDbType.NVarChar).Value = DateTime.Now;
                    command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static AuctionBid GetMaxBid(string auctionId)
        {
            AuctionBid auctionBid = null;
            using(SqlConnection connection = new SqlConnection("Server=119.59.96.90;Database=adoppix_db;User ID=adoppix_backend;Password=Bto#97p84;"))
            {
                using(SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GetMaxBidByAuction";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@AuctionId", SqlDbType.NVarChar).Value = auctionId;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        auctionBid = new AuctionBid();
                        auctionBid.Id = Convert.ToInt32(reader["auctionBidId"]);
                        auctionBid.UserId = reader["userId"].ToString();
                        auctionBid.AuctionId = reader["auctionId"].ToString();
                        auctionBid.Amount = Convert.ToInt32(reader["amount"]);
                    }
                    connection.Close();
                }
            }
            return auctionBid;
        }

        public async Task<List<string>> FindImageByAuctionId(string auctionId)
        {
            return await entityUnitOfWork.AuctionImageRepository.Find(x => x.AuctionId == auctionId)
                                                                .Select(x => x.Name)
                                                                .ToListAsync();
        }

        public async Task<List<AuctionBid>> GetBidHistories(string auctionId)
        {
            return await entityUnitOfWork.AuctionBidRepository.Find(x => x.AuctionId == auctionId)
                                                              .ToListAsync();
        }

        public async Task<List<Auction>> GetAllAsync(int take, int page, bool onlyActive)
        {
            var auctions = entityUnitOfWork.AuctionRepository.Find(x => x.IsActive == true && x.IsWon == false);

            if(onlyActive) auctions = auctions.Where(x => x.Start != null && x.Stop != null);

            return await auctions.Skip(take * page).Take(take).ToListAsync();
        }
    }
}
