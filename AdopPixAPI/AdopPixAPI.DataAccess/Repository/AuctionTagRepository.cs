using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class AuctionTagRepository : Repository<AuctionTag>, IAuctionTagRepository
    {
        public AuctionTagRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
