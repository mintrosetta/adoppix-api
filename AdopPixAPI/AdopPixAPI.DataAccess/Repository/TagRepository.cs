using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Models;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.Repository
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(SqlServerDbContext context) : base(context)
        {
        }
    }
}
