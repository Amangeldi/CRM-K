using CRM.BLL.DTO;
using CRM.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.BLL.Interfaces
{
    public interface ISingleTemp
    {
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
        bool FirstInit { get; set; }
    }
}
