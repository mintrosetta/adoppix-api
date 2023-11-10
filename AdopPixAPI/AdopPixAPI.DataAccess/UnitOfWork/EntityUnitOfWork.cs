using AdopPixAPI.DataAccess.ApplicationDbContext;
using AdopPixAPI.DataAccess.Repository;
using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.UnitOfWork
{
    public class EntityUnitOfWork : IEntityUnitOfWork, IDisposable
    {
        private readonly SqlServerDbContext context;

        public IUserRepository UserRepository { get; private set; }
        public IRoleRepository RoleRepository { get; private set; }
        public IUserProfileRepository UserProfileRepository { get; private set; }
        public IAuctionRepository AuctionRepository { get; private set; }
        public IAuctionImageRepository AuctionImageRepository { get; private set; }
        public ITagRepository TagRepository { get; private set; }
        public IAuctionTagRepository AuctionTagRepository { get; private set; }
        public IAuctionBidRepository AuctionBidRepository { get; private set; }
        public IPostRepository PostRepository { get; private set; }
        public IPostImageRepository PostImageRepository { get; private set; }
        public IPostTagRepository PostTagRepository { get; private set; }
        public IPostCommentRepository PostCommentRepository { get; private set; }
        public IPostLikeRepository PostLikeRepository { get; private set; }
        public IAuctionWonRepository AuctionWonRepository  { get; private set; }
        public IUserFollowRepository UserFollowRepository  { get; private set; }

        public EntityUnitOfWork(SqlServerDbContext context)
        {
            this.context = context;

            UserRepository = new UserRepository(context);
            RoleRepository = new RoleRepository(context);
            UserProfileRepository = new UserProfileRepository(context);
            UserFollowRepository = new UserFollowRepository(context);

            TagRepository = new TagRepository(context);

            AuctionRepository = new AuctionRepository(context);
            AuctionImageRepository = new AuctionImageRepository(context);
            AuctionTagRepository = new AuctionTagRepository(context);
            AuctionBidRepository = new AuctionBidRepository(context);
            AuctionWonRepository = new AuctionWonRepository(context);
            
            AuctionRepository = new AuctionRepository(context);
            AuctionImageRepository = new AuctionImageRepository(context);
            TagRepository = new TagRepository(context);
            AuctionTagRepository = new AuctionTagRepository(context);
            AuctionBidRepository = new AuctionBidRepository(context);

            PostRepository = new PostRepository(context);
            PostImageRepository = new PostImageRepository(context);
            PostTagRepository = new PostTagRepository(context);
            PostCommentRepository = new PostCommentRepository(context);
            PostLikeRepository = new PostLikeRepository(context);
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
