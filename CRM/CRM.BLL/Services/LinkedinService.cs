using CRM.BLL.Interfaces;
using CRM.DAL.EF;
using CRM.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Services
{
    public class LinkedinService : ILinkedinService
    {
        ApiContext db;
        public LinkedinService(ApiContext context)
        {
            db = context;
        }
        public async Task<string> AddLinkedin(string LinkedinLink)
        {
            if(LinkedinLink!=null)
            {
                if(LinkedinLink.Length>12&&LinkedinLink.Substring(0, 12) == "https://www.")
                {
                    LinkedinLink = LinkedinLink.Substring(12);
                }
                if(LinkedinLink.Length>12&&LinkedinLink.Substring(0, 12) == "linkedin.com")
                {
                    if ((db.Linkedins.Where(p=>p.FullLink==LinkedinLink).FirstOrDefault())==null)
                    {
                        Linkedin linkedin = new Linkedin
                        {
                            FullLink = LinkedinLink
                        };
                        await db.Linkedins.AddAsync(linkedin);
                        await db.SaveChangesAsync();
                        return LinkedinLink;
                    }
                }
            }
            return null;
            
        }

        public async Task<string> GetWebsiteLink(string WebsiteLink)
        {
            if(WebsiteLink!=null)
            {
                if(WebsiteLink.Length > 5 && WebsiteLink.Substring(0, 8) == "https://")
                {
                    WebsiteLink = WebsiteLink.Substring(8);
                }
                if (WebsiteLink.Length > 5 && WebsiteLink.Substring(0, 7) == "http://")
                {
                    WebsiteLink = WebsiteLink.Substring(7);
                }
            }
            return WebsiteLink;
        }
    }
}
