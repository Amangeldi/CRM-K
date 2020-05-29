using AutoMapper;
using CRM.BLL.DTO;
using CRM.BLL.Interfaces;
using CRM.DAL.EF;
using CRM.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRM.BLL.Services
{
    public class RegionService : IRegionService
    {
        readonly ApiContext db;
        public RegionService(ApiContext context)
        {
            db = context;
        }
        public async Task<Region> CreateRegion(string RegionName)
        {
            Region region = new Region
            {
                Name = RegionName
            };
            await db.Regions.AddAsync(region);
            await db.SaveChangesAsync();
            return region;
        }

        public async Task<bool> DeleteRegion(int RegionId)
        {
            Region region = await db.Regions.FindAsync(RegionId);
            if (region == null)
            {
                return false;
            }
            db.Regions.Remove(region);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<RegionDTO> EditRegion(RegionDTO RegionDTO)
        {
            Region region = await db.Regions.FindAsync(RegionDTO.Id);
            region.Name = RegionDTO.Name;
            db.Regions.Update(region);
            await db.SaveChangesAsync();
            return RegionDTO;
        }

        public async Task<RegionDTO> GetRegion(int RegionId)
        {
            try
            {
                Region Region = await db.Regions.FindAsync(RegionId);

                RegionDTO RegionDTO = new RegionDTO
                {
                    Name = Region.Name,
                };
                return RegionDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<RegionDTO>> GetRegions()
        {
            IEnumerable<Region> Regions = await db.Regions.ToListAsync();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Region, RegionDTO>()).CreateMapper();
            IEnumerable<RegionDTO> RegionDTOs = mapper.Map<IEnumerable<Region>, IEnumerable<RegionDTO>>(Regions);
            return RegionDTOs;
        }
    }
}
