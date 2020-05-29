using CRM.BLL.DTO;
using CRM.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Interfaces
{
    public interface IRegionService
    {
        Task<Region> CreateRegion(string RegionName);
        Task<RegionDTO> GetRegion(int RegionId);
        Task<IEnumerable<RegionDTO>> GetRegions();
        Task<bool> DeleteRegion(int RegionId);
        Task<RegionDTO> EditRegion(RegionDTO RegionDTO);
    }
}
