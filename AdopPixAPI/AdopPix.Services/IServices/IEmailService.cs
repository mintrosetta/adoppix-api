using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdopPixAPI.Services.IServices
{
    public interface IEmailService
    {
        string CreateTemplate(string emailType, string header, string subHeader, string url);
        Task SendAsync(string to, string subject, string body);
    }
}
