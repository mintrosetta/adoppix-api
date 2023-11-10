using Microsoft.AspNetCore.Http;

namespace AdopPixAPI.Services.IServices
{
    public interface IUploadFileService
    {
        Task Upload(IFormFile stream, string fileName);
    }
}
