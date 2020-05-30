using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.BLL.DTO
{
    public class ContactRegistrationDTO
    {
        public string Email { get; set; }
        public string Position { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string LinkedinLink { get; set; }
        public int CompanyId { get; set; }
    }
}
