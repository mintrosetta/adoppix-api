using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(SqlServerDbContext context) : base(context)
        {

        }
    }
}
