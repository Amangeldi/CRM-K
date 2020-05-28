using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.DAL.Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Country> Countries { get; set; }
    }
}

