using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.InterfaceRepositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByPhoneNumber(string phoneNumber);
        Task<User> GetUserByUserName(string userName);
        Task AddRolesToUserAsync(User user , List<string> listRoles);
        Task<IEnumerable<string>> GetRolesOfUserAsync(User user);
        Task<IEnumerable<User>> GetUsersWithRoleManagerAsync();
        Task<bool> IsUserInDesignTeamAsync(int userId);
        Task<IEnumerable<Notification>> GetNotificationByUserId(int userId);
        Task<IEnumerable<Permissions>> GetPermissionsByUserId(int id);
    }
}
