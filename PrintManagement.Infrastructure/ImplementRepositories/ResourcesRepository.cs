using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using PrintManagement.Infrastructure.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Infrastructure.ImplementRepositories
{
    public class ResourcesRepository : IResourcesRepository
    {
        private readonly ApplicationDbContext _context;

        public ResourcesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Resources> GetResourcesByName(string name)
        {
            var resource = await _context.Resources.FirstOrDefaultAsync(x => x.ResourceName.ToLower() == name.ToLower());
            return resource;
        }
        public async Task UpdateAvailableQuantityAsync(int resourceId)
        {
            // Tính tổng Quantity từ ResourceProperty với ResourceId = resourceId
            var totalQuantity = await _context.ResourceProperties
                .Where(rp => rp.ResourceId == resourceId)
                .SumAsync(rp => rp.Quantity);

            // Lấy đối tượng Resource cần cập nhật
            var resource = await _context.Resources
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource != null)
            {
                // Cập nhật AvailableQuantity trong Resources
                resource.AvailableQuantity = totalQuantity;

                // Lưu thay đổi
                await _context.SaveChangesAsync();
            }
        }
    }
}
