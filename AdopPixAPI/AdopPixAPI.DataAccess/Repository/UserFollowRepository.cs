using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class UserFollowRepository : Repository<UserFollow>, IUserFollowRepository
    {
        public UserFollowRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
