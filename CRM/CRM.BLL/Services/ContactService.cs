using CRM.BLL.DTO;
using CRM.BLL.Interfaces;
using CRM.DAL.EF;
using CRM.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Services
{
    public class ContactService : IContactService
    {
        ILinkedinService linkedinServ;
        ApiContext db;
        public ContactService(ILinkedinService linkedinService, ApiContext context)
        {
            linkedinServ = linkedinService;
            db = context;
        }
        public async Task AddCompanyContact(ContactRegistrationDTO CompanyContact)
        {
            Contact newContact;
            if (CompanyContact.LinkedinLink!=null)
            {
                CompanyContact.LinkedinLink = await linkedinServ.AddLinkedin(CompanyContact.LinkedinLink);
            }
            if(CompanyContact.LinkedinLink!=null)
            {
                Linkedin linkedin = await db.Linkedins.Where(p => p.FullLink == CompanyContact.LinkedinLink).FirstOrDefaultAsync();
                newContact = new Contact
                {
                    Email = CompanyContact.Email,
                    FirstName = CompanyContact.FirstName,
                    Linkedin = linkedin,
                    Surname = CompanyContact.Surname,
                    Position = CompanyContact.Position
                };
            }
            else
            {
                newContact = new Contact
                {
                    Email = CompanyContact.Email,
                    FirstName = CompanyContact.FirstName,
                    Surname = CompanyContact.Surname,
                    Position = CompanyContact.Position
                };
            }
            await db.Contacts.AddAsync(newContact);
            Company company = await db.Companies.FindAsync(CompanyContact.CompanyId);
            CompanyContactLink companyContactLink = new CompanyContactLink
            {
                CompanyId = company.Id,
                Contact = newContact
            };
            await db.CompanyContactLinks.AddAsync(companyContactLink);
            await db.SaveChangesAsync();

        }
    }
}
