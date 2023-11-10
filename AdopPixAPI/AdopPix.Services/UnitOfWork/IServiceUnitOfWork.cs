using AdopPix.Services.IServices;
using AdopPixAPI.Services.IServices;

namespace AdopPixAPI.Services.UnitOfWork
{
    public interface IServiceUnitOfWork
    {
        public IUserService UserService { get; }
        public IEmailService EmailService { get; }
        public ITokenService TokenService { get; }
        public IUploadFileService UploadFileService { get; }
        public IAuctionService AuctionService { get; }
        public IPostService PostService { get; }
        public ITagService TagService { get; }
    }
}
