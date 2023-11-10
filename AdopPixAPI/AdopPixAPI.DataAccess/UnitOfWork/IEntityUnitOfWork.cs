using AdopPixAPI.DataAccess.Repository.IRepository;

namespace AdopPixAPI.DataAccess.UnitOfWork
{
    public interface IEntityUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IUserProfileRepository UserProfileRepository { get; }
        public IUserFollowRepository UserFollowRepository { get; }

        public IAuctionRepository AuctionRepository { get; }
        public IAuctionImageRepository AuctionImageRepository { get; }
        public IAuctionTagRepository AuctionTagRepository { get; }
        public IAuctionBidRepository AuctionBidRepository { get; }
        public IAuctionWonRepository AuctionWonRepository { get; }

        public IPostRepository PostRepository { get; }
        public IPostImageRepository PostImageRepository { get; }
        public IPostTagRepository PostTagRepository { get; }
        public IPostLikeRepository PostLikeRepository { get; }
        public IPostCommentRepository PostCommentRepository { get; }

        public ITagRepository TagRepository { get; }

        Task CompleteAsync();
    }
}
