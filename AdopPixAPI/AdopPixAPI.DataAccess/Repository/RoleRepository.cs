using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class RoleRepository : Repository<UserRole>, IRoleRepository
    {
        public RoleRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
