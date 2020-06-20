using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CRM.BLL.DTO
{
    public class AdvertisingCompanyStatisticDTO
    {
        [Name("email")]
        public string Email { get; set; }
        [Name("firstName")]
        public string FirstName { get; set; }
        [Name("lastName")]
        public string LastName { get; set; }
        [Name("picture")]
        public string Picture { get; set; }
        [Name("phone")]
        public string Phone { get; set; }
        [Name("linkedinUrl")]
        public string LinkedinUrl { get; set; }
        [Name("Title")]
        public string Title { get; set; }
        [Name("Company website")]
        public string CompanyWebsite { get; set; }
        [Name("Full Name")]
        public string FullName { get; set; }
        [Name("Surname")]
        public string Surname { get; set; }
        [Name("notes")]
        public string Notes { get; set; }
        [Name("sentStep")]
        public string SentStep { get; set; }
        [Name("sentAt")]
        public string SentAt { get; set; }
        [Name("repliedAt")]
        public string RepliedAt { get; set; }
        [Name("openedAt")]
        public string OpenedAt { get; set; }
        [Name("bouncedAt")]
        public string BouncedAt { get; set; }
        [Name("unsubscribedAt")]
        public string UnsubscribedAt { get; set; }
        [Name("clickedAt")]
        public string ClickedAt { get; set; }
    }
}
