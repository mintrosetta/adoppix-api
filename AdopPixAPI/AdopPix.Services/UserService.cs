using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.UnitOfWork;
using AdopPixAPI.DTOs;
using AdopPixAPI.Services.IServices;
using AdopPixAPI.Services.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AdopPixAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration configuration;
        private readonly IEntityUnitOfWork entityUnitOfWork;
        private readonly ITokenService tokenService;
        private readonly IUploadFileService uploadFileService;

        public UserService(IConfiguration configuration, 
                           IEntityUnitOfWork entityUnitOfWork,
                           ITokenService tokenService,
                           IUploadFileService uploadFileService)
        {
            this.configuration = configuration;
            this.entityUnitOfWork = entityUnitOfWork;
            this.tokenService = tokenService;
            this.uploadFileService = uploadFileService;
        }

        public async Task<User> FindById(string userId)
        {
            return await entityUnitOfWork.UserRepository.Find(user => user.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<User> FindByEmail(string email)
        {
            return await entityUnitOfWork.UserRepository.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> FindByUsername(string username)
        {
            var profile = await entityUnitOfWork.UserProfileRepository.Find(x => x.Username == username).FirstOrDefaultAsync();
            if(profile == null) return null;

            return await entityUnitOfWork.UserRepository.FindById(profile.UserId);
        }

        public async Task<User> Create(UserRegisterDto userRegisterDto)
        {
            using var hmac = new HMACSHA512();
            string userId = Guid.NewGuid().ToString();
            DateTime created = DateTime.UtcNow;

            if (await entityUnitOfWork.RoleRepository.Find(role => role.Name == "member").FirstOrDefaultAsync() == null)
            {
                UserRole roleCreate = new UserRole()
                {
                    Name = "member"
                };

                await entityUnitOfWork.RoleRepository.Add(roleCreate);
                await entityUnitOfWork.CompleteAsync();
            }

            UserRole role = await entityUnitOfWork.RoleRepository.Find(role => role.Name == "member").FirstOrDefaultAsync();

            User user = new User()
            {
                Id = userId,
                Email = userRegisterDto.Email,
                IsConfirmEmail = false,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDto.Password)),
                PasswordSalt = hmac.Key,
                PhoneNumber = null,
                IsConfirmPhoneNumber = false,
                Created = created,
                Updated = null,
                IsActive = true,
                RoleId = role.Id,
            };

            await entityUnitOfWork.UserRepository.Add(user);

            UserProfile profile = new UserProfile()
            {
                UserId = userId,
                Username = userRegisterDto.Username,
                Created = created,
                Money = 0,
                IsDark = false
            };

            await entityUnitOfWork.UserProfileRepository.Add(profile);
            await entityUnitOfWork.CompleteAsync();
            return user;
        }

        public async Task<User> Verify(UserLoginDto userLoginDto)
        {
            User user = await FindByEmail(userLoginDto.Email);
            if (user == null) return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userLoginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return null;
            }

            return user;
        }

        public async Task<UserProfileDto> FindProfileById(string userId)
        {
            var userProfile = await entityUnitOfWork.UserProfileRepository.Find(profile => profile.UserId == userId)
                                                                          .Select(profile => new UserProfileDto()
                                                                          {
                                                                              Username = profile.Username,
                                                                              CoverImage = profile.CoverImage,
                                                                              Description = profile.Description,
                                                                              ProfileImage = profile.ProfileImage,
                                                                          })
                                                                          .FirstOrDefaultAsync();
            return userProfile;
        }

        public async Task<UserProfileDto> FindProfileByUsername(string username)
        {
            var userProfile = await entityUnitOfWork.UserProfileRepository.Find(profile => profile.Username == username)
                                                                          .Select(profile => new UserProfileDto()
                                                                          {
                                                                              Username = profile.Username,
                                                                              CoverImage = profile.CoverImage,
                                                                              Description = profile.Description,
                                                                              ProfileImage = profile.ProfileImage,
                                                                          })
                                                                          .FirstOrDefaultAsync();
            return userProfile;
        }

        public async Task UpdateProfileByUserId(UpdateUserProfileDto dto, string userId)
        {
            var currentUserProfile = await entityUnitOfWork.UserProfileRepository.FindByUserId(userId);
            if (currentUserProfile != null)
            {
                if(dto.ProfileImage != null)
                {
                    string profileName = Guid.NewGuid().ToString();
                    await uploadFileService.Upload(dto.ProfileImage, profileName);
                    var ext = Path.GetExtension(dto.ProfileImage.FileName) ;
                    currentUserProfile.ProfileImage = profileName + ext;
                }

                if(dto.CoverImage != null)
                {
                    string coverName = Guid.NewGuid().ToString();
                    await uploadFileService.Upload(dto.CoverImage, coverName);
                    var ext = Path.GetExtension(dto.CoverImage.FileName) ;
                    currentUserProfile.CoverImage = coverName+ext;
                }
                
                if(dto.Description != null) currentUserProfile.Description = dto.Description;
                if(dto.Username != null) currentUserProfile.Username = dto.Username;

                currentUserProfile.Updated = DateTime.Now;

                entityUnitOfWork.UserProfileRepository.Update(currentUserProfile);
                await entityUnitOfWork.CompleteAsync();
            }
        }

        public string CreateAccessToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            };
            string token = tokenService.CreateToken(configuration.GetSection("TokenKey:Access").Value, claims, DateTime.Now.AddDays(1));
            return token;
        }

        public string CreateConfirmEmailToken(User user)
        {
            string plainTexts = $"{user.Id},{user.Email}";
            string token = tokenService.EncodeBase64(plainTexts);
            return token;
        }

        public async Task ConfirmEmail(User user)
        {
            entityUnitOfWork.UserRepository.Update(user);
            await entityUnitOfWork.CompleteAsync();
        }

        public string CreateChangeEmailToken(User user, string newEmail)
        {
            string plainTexts = $"{user.Id},{user.Email},{newEmail}";
            string token = tokenService.EncodeBase64(plainTexts);
            return token;
        }

        public async Task ConfirmChangeEmail(User user)
        {
            entityUnitOfWork.UserRepository.Update(user);
            await entityUnitOfWork.CompleteAsync();
        }

        public async Task ConfirmChangePassword(User user, string newPassword)
        {

            using var hmac = new HMACSHA512(user.PasswordSalt);
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
            user.Updated = DateTime.Now;

            entityUnitOfWork.UserRepository.Update(user);

            await entityUnitOfWork.CompleteAsync();
        }

        public async Task<decimal> FindMoneyById(string userId)
        {
            return await entityUnitOfWork.UserProfileRepository.Find(x => x.UserId == userId).Select(x => x.Money).FirstOrDefaultAsync();
        }
    
        public async Task<bool> Follow(string userId, string userFollowId) 
        {
            var follow = await entityUnitOfWork.UserFollowRepository.Find(x => x.FromUserId == userId && x.ToUserId == userFollowId)
                                                                    .FirstOrDefaultAsync();
            bool result;
            if(follow == null) 
            {
                UserFollow userFollow = new UserFollow()
                {
                    FromUserId = userId,
                    ToUserId = userFollowId,
                    Created = DateTime.Now
                };
                await entityUnitOfWork.UserFollowRepository.Add(userFollow);
                result = true;
            } 
            else 
            {
                entityUnitOfWork.UserFollowRepository.Remove(follow);
                result = false;
            }

            await entityUnitOfWork.CompleteAsync();
            return result;
        }
    
        public async Task<List<string>> FindFollowByUserId(string userId)
        {
            return await entityUnitOfWork.UserFollowRepository.Find(x => x.FromUserId == userId)
                                                              .Select(x => x.ToUserId)
                                                              .ToListAsync();
        }

        public async Task<UserFollow> IsFollow(string fromUserId, string toUserId)
        {
            return await entityUnitOfWork.UserFollowRepository.Find(x => x.FromUserId == fromUserId && x.ToUserId == toUserId)
                                                              .FirstOrDefaultAsync();
        }

        public async Task Transaction(string userId, decimal amount)
        {
            var userProfile = await entityUnitOfWork.UserProfileRepository.Find(x => x.UserId == userId).FirstOrDefaultAsync();
            userProfile.Money += amount;
            entityUnitOfWork.UserProfileRepository.Update(userProfile);
            await entityUnitOfWork.CompleteAsync();
        }

        public async Task<List<string>> Search(string search)
        {
            var users = await entityUnitOfWork.UserProfileRepository.Find(x => x.Username.ToLower().Contains(search.ToLower())).ToListAsync();
            return users.Select(x => x.Username).ToList();
        }
    }
}
