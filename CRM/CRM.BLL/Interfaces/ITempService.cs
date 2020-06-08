using CRM.BLL.DTO;
using CRM.DAL.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Interfaces
{
    public interface ITempService
    {
        /*
        IEnumerable<CompanyDTO> AllCompanies { get; set; }
        IEnumerable<CompanyDTO> NewCompanies { get; set; }
        IEnumerable<CompanyDTO> QualifiedCompanies { get; set; }
        IEnumerable<CompanyDTO> NotQualifiedCompanies { get; set; }
        IEnumerable<ContactDTO> Contacts { get; set; }
        IEnumerable<CompanyModel> CompanyModels { get; set; }
        IEnumerable<CompanyContactLink> CompanyContactLinks { get; set; }
        IEnumerable<Linkedin> Linkedins { get; set; }
        IEnumerable<CountryDTO> Countries { get; set; }
        IEnumerable<LogDTO> Logs { get; set; }
        IEnumerable<RegionDTO> Regions { get; set; }
        bool FirstInit { get; set; }*/
        GetUserDTO CurrentUser { get; set; }
        Task UpdateCompanies();
        Task UpdateNewCompanies();
        Task UpdateQualifiedCompanies();
        Task UpdateNotQualifiedCompanies();
        Task UpdateLinkedins();
        Task UpdateCompanyContactLinks();
        Task UpdateCountries();
        Task UpdateContacts();
        Task UpdateRegions();
        Task UpdateCompanyModels();
        Task UpdateLogs();
        void SetId(int Id);
        int GetSelectedId();
        Task UpdateAllTemp();
        Task<IEnumerable<ContactDTO>> GetCompanyContacts(int CompanyId);
        IEnumerable<CompanyDTO> GetPage(IEnumerable<CompanyDTO> list, int page, int pageSize);
    }
}
