using CRM.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Interfaces
{
    public interface IContactService
    {
        Task AddCompanyContact(ContactRegistrationDTO CompanyContact);
    }
}
