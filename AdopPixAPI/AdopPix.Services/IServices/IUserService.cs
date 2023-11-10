using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DTOs;

namespace AdopPixAPI.Services.IServices
{
    public interface IUserService
    {
        Task<User> FindById(string userId);
        Task<User> FindByEmail(string email);
        Task<User> FindByUsername(string username);
        Task<User> Create(UserRegisterDto userRegisterDto);
        Task<User> Verify(UserLoginDto userLoginDto);
        Task<UserProfileDto> FindProfileById(string userId);
        Task<UserProfileDto> FindProfileByUsername(string username);
        Task UpdateProfileByUserId(UpdateUserProfileDto updateUserProfileDto, string userId);

        string CreateAccessToken(User user);
        string CreateConfirmEmailToken(User user);
        Task ConfirmEmail(User user);
        string CreateChangeEmailToken(User user, string newEmail);
        Task ConfirmChangeEmail(User user);
        Task ConfirmChangePassword(User user, string newPassword);
        Task<decimal> FindMoneyById(string userId);
        Task<bool> Follow(string userId, string userFollowId);
        Task<List<string>> FindFollowByUserId(string userId);
        Task<UserFollow> IsFollow(string fromUserId, string toUserId);
        Task Transaction(string userId, decimal amount);
        Task<List<string>> Search(string search);
    }
}
