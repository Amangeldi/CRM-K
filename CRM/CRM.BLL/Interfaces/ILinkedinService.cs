using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Interfaces
{
    public interface ILinkedinService
    {
        Task<string> AddLinkedin(string LinkedinLink);
        Task<string> GetWebsiteLink(string WebsiteLink);
    }
}
