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
        readonly ApiContext db;
        public CsvService(ICompanyService companyService, ApiContext context, ISingleTemp singleTemp)
        {
            _companyService = companyService;
            _singleTemp = singleTemp;
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
            foreach (var company in Companies ?? Enumerable.Empty<CompanyCsvModel>())
            {
                List<Region> regions = await db.Regions.Where(p => p.Name == company.RegionName).ToListAsync();
                Region region;
                if (regions.Count==0)
                {
                    region = new Region { Name = company.RegionName };
                    await db.Regions.AddAsync(region);
                }
                else
                {
                    region = regions.First();
                }
                CountryDTO countryDTO = _singleTemp.Countries.Where(p => p.RegionId == region.Id&&p.Name==company.HGBasedInCountry).FirstOrDefault();
                Country country;
                if (countryDTO == null)
                {
                    country = new Country { Name = company.HGBasedInCountry, RegionId = region.Id};
                    await db.Countries.AddAsync(country);
                }
                else
                {
                    country = new Country
                    {
                        Id = countryDTO.Id,
                        Capital = countryDTO.Capital,
                        Name = countryDTO.Name,
                        RegionId = countryDTO.RegionId
                    };
                }
                CompanyRegistrationDTO newCompany = new CompanyRegistrationDTO
                {
                    CompanyLegalName = company.CompanyLegalName,
                    TradingName = company.TradingName,
                    Website = company.Website,
                    CompanyLinkedinLink = company.CompanyLinkedinLink,
                    HGBasedInCountryId = country.Id
                };
                try
                {
                    await _companyService.CreateCompany(newCompany);
                }
                catch(Exception ex)
                {

                }
            }
            await db.SaveChangesAsync();
        }
    }
}
