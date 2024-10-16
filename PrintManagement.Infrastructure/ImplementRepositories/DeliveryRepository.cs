using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using PrintManagement.Domain.Models;
using PrintManagement.Infrastructure.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Infrastructure.ImplementRepositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly ApplicationDbContext _context;

        public DeliveryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeliveryModels>> GetAllDeliveryByStatus(DStatus? dStatus)
        {
            // Truy vấn ban đầu từ bảng Delivery
            var query = from delivery in _context.Deliveries
                        join project in _context.Projects on delivery.ProjectId equals project.Id
                        join customer in _context.Customers on delivery.CustomerId equals customer.Id
                        join deliverer in _context.Users on delivery.DeliverId equals deliverer.Id
                        join shippingMethod in _context.ShippingMethods on delivery.ShippingMethodId equals shippingMethod.Id
                        where !dStatus.HasValue || delivery.DeliveryStatus == dStatus.Value // Kiểm tra nếu có điều kiện dStatus
                        select new DeliveryModels
                        {
                            Id = delivery.Id,
                            ShippingMethodId = delivery.ShippingMethodId,
                            CustomerId = delivery.CustomerId,
                            DeliverId = delivery.DeliverId,
                            ProjectId = delivery.ProjectId,
                            DeliveryAddress = delivery.DeliveryAddress,
                            EstimateDeliveryTime = delivery.EstimateDeliveryTime,
                            ActualDeliveryTime = delivery.ActualDeliveryTime,
                            DeliveryStatus = delivery.DeliveryStatus,
                            // Chỉ lấy các trường cần thiết từ các bảng liên quan
                            ShippingMethodName = shippingMethod.ShippingMethodName,
                            ProjectName = project.ProjectName,
                            CustomeName = customer.FullName,
                            DeliverName = deliverer.FullName
                        };

            // Thực hiện truy vấn và trả về kết quả
            return await query.ToListAsync();
        }
        public async Task<int?> GetShipperIdByDeliveryAddressAsync(string deliveryAddress)
        {
            // Tìm kiếm bản ghi Delivery có trạng thái Processing và địa chỉ giao hàng khớp
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.DeliveryStatus == DStatus.Processing && d.DeliveryAddress == deliveryAddress);

            if (delivery == null)
            {
                return null;
            }

            return delivery.DeliverId;
        }
        public async Task<IEnumerable<Project>> GetCompletedProjectsNotInDeliveryAsync()
        {
            var completedProjects = await _context.Projects
                .Where(p => p.ProjectStatus == PStatus.Completed)
                .Where(p => !_context.Deliveries.Any(d => d.ProjectId == p.Id))
                .ToListAsync();

            return completedProjects;
        }
        public async Task<IEnumerable<User>> GetEmployeesInDeliveryTeamAsync()
        {
            var usersInDeliveryTeamWithEmployeeRole = await (from u in _context.Users
                                                             join p in _context.Permissions on u.Id equals p.UserId
                                                             join r in _context.Roles on p.RoleId equals r.Id
                                                             join t in _context.Teams on u.TeamId equals t.Id
                                                             where t.Name.ToLower() == "delivery" && r.RoleCode == "Employee"
                                                             select u)
                                                            .ToListAsync();

            return usersInDeliveryTeamWithEmployeeRole;
        }

        public async Task<IEnumerable<DeliveryModels>> GetAllDeliveryByDeliverIdAndStatus(int deliverId, DStatus? dStatus)
        {
            // Truy vấn cơ bản từ bảng Delivery
            var query = from delivery in _context.Deliveries
                        join project in _context.Projects on delivery.ProjectId equals project.Id
                        join customer in _context.Customers on delivery.CustomerId equals customer.Id
                        join shippingMethod in _context.ShippingMethods on delivery.ShippingMethodId equals shippingMethod.Id
                        // Điều kiện lọc theo DeliverId và trạng thái (nếu có)
                        where delivery.DeliverId == deliverId && (!dStatus.HasValue || delivery.DeliveryStatus == dStatus.Value)
                        select new DeliveryModels
                        {
                            Id = delivery.Id,
                            ShippingMethodId = delivery.ShippingMethodId,
                            CustomerId = delivery.CustomerId,
                            DeliverId = delivery.DeliverId,
                            ProjectId = delivery.ProjectId,
                            DeliveryAddress = delivery.DeliveryAddress,
                            EstimateDeliveryTime = delivery.EstimateDeliveryTime,
                            ActualDeliveryTime = delivery.ActualDeliveryTime,
                            DeliveryStatus = delivery.DeliveryStatus,
                            // Chỉ lấy các trường cần thiết từ các bảng liên quan
                            ShippingMethodName = shippingMethod.ShippingMethodName,
                            ProjectName = project.ProjectName,
                            CustomeName = customer.FullName,
                        };

            // Thực hiện truy vấn và trả về kết quả
            return await query.ToListAsync();
        }
    }
}
