using AdopPixAPI.DataAccess.Models;

namespace AdopPixAPI.DataAccess.Repository.IRepository
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        Task<UserProfile> FindByUserId(string userId);
    }
}
