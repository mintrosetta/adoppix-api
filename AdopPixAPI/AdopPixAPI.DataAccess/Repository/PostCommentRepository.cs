using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class PostCommentRepository : Repository<PostComment>, IPostCommentRepository
    {
        public PostCommentRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
