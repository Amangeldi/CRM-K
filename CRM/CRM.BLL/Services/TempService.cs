using CRM.BLL.DTO;
using CRM.BLL.Interfaces;
using CRM.DAL.EF;
using CRM.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Services
{
    public class TempService : ITempService
    {
        IMailFindService mailFindServ;
        ICompanyService companyServ;
        ILogService _logService;
        ApiContext db;
        ICountryService countryServ;
        IUserRegistrationService userRegistrationServ;
        IServiceScopeFactory _ServiceScopeFactory;
        IRegionService regionServ;
        public TempService(IMailFindService mailFindService, ICompanyService companyService,
            ILogService logService, ApiContext context, IUserRegistrationService userRegistrationService,
            ICountryService countryService, IServiceScopeFactory ServiceScopeFactory,
            IRegionService regionService)
        {
            mailFindServ = mailFindService;
            companyServ = companyService;
            _logService = logService;
            db = context;
            countryServ = countryService;
            userRegistrationServ = userRegistrationService;
            _ServiceScopeFactory = ServiceScopeFactory;
            regionServ = regionService;
        }
        public GetUserDTO CurrentUser { get; set; }
        public IEnumerable<CompanyDTO> AllCompanies { get; set; }
        public IEnumerable<ContactDTO> Contacts { get; set; }
        public IEnumerable<CompanyContactLink> CompanyContactLinks { get; set; }
        public IEnumerable<Linkedin> Linkedins { get; set; }
        public IEnumerable<LogDTO> Logs { get; set; }
        public IEnumerable<CompanyDTO> NewCompanies { get; set; }
        public IEnumerable<CompanyDTO> QualifiedCompanies { get; set; }
        public IEnumerable<CompanyDTO> NotQualifiedCompanies { get; set; }
        public IEnumerable<CompanyModel> CompanyModels { get; set; }
        public IEnumerable<CountryDTO> Countries { get; set; }
        public IEnumerable<RegionDTO> Regions { get; set; }
        private int SelectedId { get; set; }
        public async Task<IEnumerable<ContactDTO>> GetCompanyContacts(int CompanyId)
        {
            IEnumerable<CompanyContactLink> _companyContactLinks = CompanyContactLinks.Where(p => p.CompanyId == CompanyId);
            List<ContactDTO> contacts = new List<ContactDTO>();
            foreach (var companyContact in _companyContactLinks)
            {
                ContactDTO contactDTO = await mailFindServ.Map(companyContact.Contact);
                contacts.Add(contactDTO);
            }
            return contacts;
        }

        public int GetSelectedId()
        {
            return SelectedId;
        }

        public void SetId(int Id)
        {
            SelectedId = Id;
        }

        public async Task UpdateAllTemp()
        {
            await UpdateCompanies();
            await UpdateNewCompanies();
            await UpdateQualifiedCompanies();
            await UpdateNotQualifiedCompanies();
            await UpdateCompanyContactLinks();
            await UpdateCountries();
            await UpdateContacts();
            await UpdateRegions();
            await UpdateCompanyModels();


            await UpdateLogs();
            
        }
        public async Task UpdateLogs()
        {
            using (var scope = _ServiceScopeFactory.CreateScope())
            {
                var LogService = scope.ServiceProvider.GetService<ILogService>();
                //var _tempService = scope.ServiceProvider.GetService<ITempService>();
                Logs = await LogService.GetLogs();
            }
        }
        public async Task UpdateCompanies()
        {
            AllCompanies = await companyServ.GetCompanies();
        }
        public async Task UpdateNewCompanies()
        {
           NewCompanies = await companyServ.GetNewCompanies();
        }
        public async Task UpdateQualifiedCompanies()
        {
            QualifiedCompanies = await companyServ.GetQualifiedCompanies();
        }
        public async Task UpdateNotQualifiedCompanies()
        {
            NotQualifiedCompanies = await companyServ.GetNotQualifiedCompanies();
        }
        public async Task UpdateLinkedins()
        {
            Linkedins = await db.Linkedins.ToListAsync();
        }
        public async Task UpdateCompanyContactLinks()
        {
            CompanyContactLinks = await mailFindServ.GetCompanyContactLinks();
        }
        public async Task UpdateCountries()
        {
            Countries = await countryServ.GetCountries();
        }
        public async Task UpdateContacts()
        {
            Contacts = await mailFindServ.GetAllContacts();
        }
        public async Task UpdateRegions()
        {
            Regions = await regionServ.GetRegions();
        }
        public async Task UpdateCompanyModels()
        {
            List<CompanyModel> companies = new List<CompanyModel>();
            await Task.Run(async () =>
            {
                foreach (var company in await companyServ.GetCompanies())
                {
                    var Qualifications = await db.CompanyQualifications.ToListAsync();
                    var QualificationName = Qualifications.Where(p => p.Id == company.QualificationId).FirstOrDefault().QualificationName;
                    var lead = await userRegistrationServ.GetUserFullName(company.LeadOwnerId);
                    var country = await countryServ.GetCountry(company.HGBasedInCountryId);

                    string linkedinLink = null;
                    if (company.CompanyLinkedinId != 0)
                    {
                        var linkedin = await db.Linkedins.FindAsync(company.CompanyLinkedinId);
                        linkedinLink = linkedin.FullLink;

                    }
                    CompanyModel companyModel = new CompanyModel
                    {
                        CompanyLegalName = company.CompanyLegalName,
                        HGBasedInCountryName = country.Name,
                        Id = company.Id,
                        LeadOwnerFullName = lead,
                        QualificationName = QualificationName,
                        QualifiedDate = company.QualifiedDate,
                        TradingName = company.TradingName,
                        Website = company.Website,
                        CompanyLinkedinFullLink = linkedinLink
                    };
                    companies.Add(companyModel);
                }
            }
            );
            CompanyModels = companies;
            /*IEnumerable<CompanyDTO> companies = await companyServ.GetCompanies();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CompanyDTO, CompanyModel>()
            .ForMember(p=>p.HGBasedInCountryName, p=>p.MapFrom(s=>countryServ.GetCountry(s.HGBasedInCountryId).Result.Name))
            .ForMember(p => p.LeadOwnerFullName, p => p.MapFrom(s =>  userRegistrationServ.GetUserFullName(s.LeadOwnerId)))
            .ForMember(p => p.QualificationName, p => p.MapFrom(s => qualificationServ.GetQualifications().Result.Where(p=>p.Id==s.QualificationId).FirstOrDefault().QualificationName))
            ).CreateMapper();
            companyModels  = mapper.Map<IEnumerable<CompanyDTO>, IEnumerable<CompanyModel>>(companies);*/
        }
    }
}
