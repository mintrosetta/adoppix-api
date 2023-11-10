using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class PostTagRepository : Repository<PostTag>, IPostTagRepository
    {
        public PostTagRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
