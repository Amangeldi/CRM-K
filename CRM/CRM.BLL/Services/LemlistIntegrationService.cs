﻿using CRM.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CRM.DAL.Entities;
using System.Linq;
using CRM.BLL.DTO;
using CRM.DAL.EF;
using CsvHelper;
using System.IO;
using System.Globalization;

namespace CRM.BLL.Services
{
    public class LemlistIntegrationService : ILemlistIntegrationService
    {
        ITempService _tempService;
        ISingleTemp _singleTemp;
        public LemlistIntegrationService(ITempService tempService, ISingleTemp singleTemp)
        {
            _tempService = tempService;
            _singleTemp = singleTemp;
        }
        //byte[] authenticationBytes = Encoding.ASCII.GetBytes(":5ae614888a4753f07b9833abad6f5b5d"); // <username>:<password>
        byte[] authenticationBytes = Encoding.ASCII.GetBytes(":30643e04a6c442702adaed7d6d921aa5");
        public async Task<IEnumerable<AddLeadInCampaignResult>> AddLeadsInCampaign(List<ContactDTO> contacts)
        {
            AdvertisingCompany advertisingCompany = (await GetAdvertisingCompanies()).FirstOrDefault();
            using HttpClient Http = new HttpClient();
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authenticationBytes));
            List<AddLeadInCampaignResult> addLeadInCampaignResponses = new List<AddLeadInCampaignResult>();
            foreach (var contact in contacts)
            {
                //string myJson = "{'firstName':'"+contact.FirstName+ "','lastName':'" + contact.Surname + "','companyName':'test company'}";
                CompanyContactLink companyContactLink = _singleTemp.CompanyContactLinks.Where(p => p.ContactId == contact.Id).FirstOrDefault();

                string myJson = "{\"firstName\":\"" + contact.FirstName + "\",\"lastName\":\"" + contact.Surname + "\",\"companyName\":\""+companyContactLink.Company.CompanyLegalName+"\"}";
                HttpContent httpContent =new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await Http.PostAsync("https://api.lemlist.com/api/campaigns/" + advertisingCompany.Id + "/leads/" + contact.Email, httpContent);
                AddLeadInCampaignResult addLeadInCampaignResponse;
                if (response.IsSuccessStatusCode)
                {
                    addLeadInCampaignResponse = new AddLeadInCampaignResult
                    {
                        Email = contact.Email,
                        Result = true
                    };
                }
                else
                {
                    addLeadInCampaignResponse = new AddLeadInCampaignResult
                    {
                        Email = contact.Email,
                        Result = false
                    };
                }
                addLeadInCampaignResponses.Add(addLeadInCampaignResponse);
            }

            return addLeadInCampaignResponses;
        }

        public async Task<IEnumerable<AdvertisingCompany>> GetAdvertisingCompanies()
        {
            using HttpClient Http = new HttpClient();
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authenticationBytes));
            var response = await Http.GetAsync("https://api.lemlist.com/api/campaigns");
            List<AdvertisingCompany> advertisingCompanies = new List<AdvertisingCompany>();
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                advertisingCompanies = JsonSerializer.Deserialize<IEnumerable<AdvertisingCompany>>(result).ToList();
            }
            return advertisingCompanies;
        }

        public async Task<IEnumerable<AdvertisingCompanyStatisticDTO>> GetAdvertisingCompanyStatistics(string advertisingCompanyId)
        {
            using HttpClient Http = new HttpClient();
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authenticationBytes));
            var response = await Http.GetAsync("https://api.lemlist.com/api/campaigns/"+advertisingCompanyId+"/export");
            List<AdvertisingCompanyStatisticDTO> advertisingCompanyStatisticDTOs = new List<AdvertisingCompanyStatisticDTO>();
            if(response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                byte[] byteArray = Encoding.ASCII.GetBytes(result);
                MemoryStream stream = new MemoryStream(byteArray);
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        csvReader.Configuration.Delimiter = ",";
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.MissingFieldFound = null;
                        advertisingCompanyStatisticDTOs = csvReader.GetRecords<AdvertisingCompanyStatisticDTO>().ToList();
                    }
                }
                //advertisingCompanyStatisticDTOs = JsonSerializer.Deserialize<IEnumerable<AdvertisingCompanyStatisticDTO>>(result).ToList();
            }
            return advertisingCompanyStatisticDTOs;
        }
    }
}
