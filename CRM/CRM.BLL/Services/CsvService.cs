using CRM.BLL.DTO;
using CRM.BLL.Interfaces;
using CRM.DAL.EF;
using CRM.DAL.Entities;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Services
{
    public class CsvService : ICsvService
    {
        const string CSV_PATH = "wwwroot/files/file.csv";
        readonly ICompanyService _companyService;
        ISingleTemp _singleTemp;
        ITempService _tempService;
        IRegionService _regionService;
        ICountryService _countryService;
        readonly ApiContext db;
        public CsvService(ICompanyService companyService, ApiContext context, ISingleTemp singleTemp,
            ITempService tempService, IRegionService regionService, ICountryService countryService)
        {
            _companyService = companyService;
            _singleTemp = singleTemp;
            _tempService = tempService;
            _regionService = regionService;
            _countryService = countryService;
            db = context;
        }
        public async Task ExportCSV(IEnumerable<CompanyDTO> companies)
        {
            throw new NotImplementedException();
        }

        public async Task ImportCSV()
        {
            List<CompanyCsvModel> Companies;
            using (StreamReader streamReader = new StreamReader(CSV_PATH))
            {
                using CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                // указываем используемый разделитель
                csvReader.Configuration.Delimiter = ",";
                csvReader.Configuration.HeaderValidated = null;
                csvReader.Configuration.MissingFieldFound = null;
                // получаем строки
                Companies = csvReader.GetRecords<CompanyCsvModel>().ToList();
            }
            CompanyQualification NewCompanyQualification = await db.CompanyQualifications.Where(p => p.QualificationName == "NewCompany").FirstAsync();
            List<Region> oldRegions = await db.Regions.ToListAsync();
            List<Country> oldCountries = await db.Countries.Include(p => p.Region).ToListAsync();
            List<Company> oldCompanies = await db.Companies.ToListAsync();
            List<Linkedin> oldLinkedins = await db.Linkedins.ToListAsync();
            List<string> NewLagalNames = new List<string>();
            foreach (var company in Companies)
            {
                Region region = oldRegions.Where(p => p.Name == company.RegionName).FirstOrDefault();
                if (region == null)
                {
                    region = new Region
                    {
                        Name = company.RegionName
                    };
                    oldRegions.Add(region);
                    await db.Regions.AddAsync(region);
                }

                Country country = oldCountries.Where(p => p.Region == region && p.Name == company.HQBasedInCountry).FirstOrDefault();
                if (country == null )
                {
                    country = new Country
                    {
                        Name = company.HQBasedInCountry,
                        Region = region
                    };
                    oldCountries.Add(country);
                    await db.Countries.AddAsync(country);
                }
                Linkedin linkedin = null;
                if (company.CompanyLinkedinLink != null && FixLinkedinLink(company.CompanyLinkedinLink) != null)
                {
                    linkedin = oldLinkedins.Where(p => p.FullLink == FixLinkedinLink(company.CompanyLinkedinLink)).FirstOrDefault();
                    if (linkedin == null)
                    {
                        linkedin = new Linkedin
                        {
                            FullLink = FixLinkedinLink(company.CompanyLinkedinLink)
                        };
                        await db.Linkedins.AddAsync(linkedin);
                    }
                }
                Company NewCompany = oldCompanies.Where(p => p.NormalizeCompanyLegalName == company.CompanyLegalName.ToLower()).FirstOrDefault();
                
                
                if (NewCompany == null && !NewLagalNames.Contains(company.CompanyLegalName.ToLower()))
                {
                    NewLagalNames.Add(company.CompanyLegalName.ToLower());
                    if (linkedin != null)
                    {
                        NewCompany = new Company
                        {
                            Qualification = NewCompanyQualification,
                            CompanyLegalName = company.CompanyLegalName,
                            NormalizeCompanyLegalName = company.CompanyLegalName.ToLower(),
                            CompanyLinkedin = linkedin,
                            HGBasedInCountry = country,
                            TradingName = company.TradingName,
                            Website = company.Website
                        };
                    }
                    else
                    {
                        NewCompany = new Company
                        {
                            Qualification = NewCompanyQualification,
                            CompanyLegalName = company.CompanyLegalName,
                            NormalizeCompanyLegalName = company.CompanyLegalName.ToLower(),
                            HGBasedInCountry = country,
                            TradingName = company.TradingName,
                            Website = company.Website
                        };
                    }
                    await db.Companies.AddAsync(NewCompany);
                }
                else
                {

                }
            }
            await db.SaveChangesAsync();
            
            string FixLinkedinLink(string LinkedinLink)
            {
                if (LinkedinLink.Length > 12 && LinkedinLink.Substring(0, 12) == "https://www.")
                {
                    LinkedinLink = LinkedinLink.Substring(12);
                }
                if (LinkedinLink.Length > 12 && LinkedinLink.Substring(0, 12) == "linkedin.com")
                {
                    return LinkedinLink;
                }
                return null;
            }
        }

    }
}
