using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class PostLikeRepository : Repository<PostLike>, IPostLikeRepository
    {
        public PostLikeRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
