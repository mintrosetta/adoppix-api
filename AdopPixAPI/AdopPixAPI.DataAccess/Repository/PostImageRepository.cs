using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class PostImageRepository : Repository<PostImage>, IPostImageRepository
    {
        public PostImageRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
