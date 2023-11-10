using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class AuctionImageRepository : Repository<AuctionImage>, IAuctionImageRepository
    {
        public AuctionImageRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
