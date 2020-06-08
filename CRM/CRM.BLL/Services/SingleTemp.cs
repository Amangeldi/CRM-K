using CRM.BLL.DTO;
using CRM.BLL.Interfaces;
using CRM.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.BLL.Services
{
    public class SingleTemp : ISingleTemp
    {
        public IEnumerable<CompanyDTO> AllCompanies { get; set; }
        public IEnumerable<CompanyDTO> NewCompanies { get; set; }
        public IEnumerable<CompanyDTO> QualifiedCompanies { get; set; }
        public IEnumerable<CompanyDTO> NotQualifiedCompanies { get; set; }
        public IEnumerable<ContactDTO> Contacts { get; set; }
        public IEnumerable<CompanyModel> CompanyModels { get; set; }
        public IEnumerable<CompanyContactLink> CompanyContactLinks { get; set; }
        public IEnumerable<Linkedin> Linkedins { get; set; }
        public IEnumerable<CountryDTO> Countries { get; set; }
        public IEnumerable<LogDTO> Logs { get; set; }
        public IEnumerable<RegionDTO> Regions { get; set; }
        public bool FirstInit { get; set; } = true;
        public string AbsoluteUri { get; set; }
    }
}
