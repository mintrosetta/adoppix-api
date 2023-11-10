using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
