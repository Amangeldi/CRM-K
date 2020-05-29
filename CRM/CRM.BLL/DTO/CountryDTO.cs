using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CRM.BLL.DTO
{
    public class CountryDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Введите Country Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите Capital")]
        public string Capital { get; set; }
        [Required(ErrorMessage = "Выберите Регион")]
        public int RegionId { get; set; }
    }
}
