using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Repository
{
    public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
    {
        private readonly SqlServerDbContext context;

        public UserProfileRepository(SqlServerDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<UserProfile> FindByUserId(string userId)
        {
            return await context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
