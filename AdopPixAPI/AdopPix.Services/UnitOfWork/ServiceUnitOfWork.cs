using AdopPix.Services;
using AdopPix.Services.IServices;
using AdopPixAPI.DataAccess.UnitOfWork;
using AdopPixAPI.Services.IServices;
using Hangfire;
using Microsoft.Extensions.Configuration;

namespace AdopPixAPI.Services.UnitOfWork
{
    public class ServiceUnitOfWork : IServiceUnitOfWork
    {
        private readonly IConfiguration configuration;
        private readonly IEntityUnitOfWork entityUnitOfWork;
        private readonly IBackgroundJobClient backgroundJobClient;

        public IUserService UserService { get; private set; }
        public IEmailService EmailService { get; private set; }
        public ITokenService TokenService { get; private set; }
        public IUploadFileService UploadFileService { get; private set; }
        public IAuctionService AuctionService { get; private set; }
        public IPostService PostService { get; private set; }
        public ITagService TagService { get; private set; }

        public ServiceUnitOfWork(IConfiguration configuration, 
            IEntityUnitOfWork entityUnitOfWork,
            IBackgroundJobClient backgroundJobClient)
        {
            this.configuration = configuration;
            this.entityUnitOfWork = entityUnitOfWork;
            this.backgroundJobClient = backgroundJobClient;
            EmailService = new EmailService(this.configuration);
            TokenService = new TokenService();
            UploadFileService = new UploadFileService(this.configuration);
            AuctionService = new AuctionService(this.entityUnitOfWork, this.UploadFileService, this.backgroundJobClient);
            UserService = new UserService(this.configuration, 
                                          this.entityUnitOfWork, 
                                          this.TokenService, 
                                          this.UploadFileService);
            PostService = new PostService(this.entityUnitOfWork, 
                                          this.UploadFileService);
            TagService = new TagService(this.entityUnitOfWork);
        }
    }
}
