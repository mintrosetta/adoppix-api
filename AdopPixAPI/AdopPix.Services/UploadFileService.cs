using AdopPixAPI.Services.IServices;
using FluentFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;

namespace AdopPixAPI.Services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IConfiguration configuration;

        public UploadFileService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Upload(IFormFile file, string fileName)
        {
            string username = configuration.GetSection("FTP:Public:User").Value;
            string password = configuration.GetSection("FTP:Public:Password").Value;
            var ext = Path.GetExtension(file.FileName);
            FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create($"ftp://pix.adoppix.com/{fileName}{ext}");
            ftp.Credentials = new NetworkCredential(username, password);
            ftp.Method = WebRequestMethods.Ftp.UploadFile;

            using(Stream stream = ftp.GetRequestStream())
            {
                file.CopyTo(stream);
            }
        }
    }
}
