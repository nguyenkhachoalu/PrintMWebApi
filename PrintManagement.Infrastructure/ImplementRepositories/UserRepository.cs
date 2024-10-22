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
    public class UserRepository : IUserRepository
    {
        ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region String processing
        private Task<bool> CompareStringAsync(string str1, string str2)
        {
            return Task.FromResult(string.Equals(str1.ToLowerInvariant(), str2.ToLowerInvariant()));
        }
        private async Task<bool> IsStringInListAsync(string inputString, List<string> listString)
        {
            if(inputString == null)
            {
                throw new ArgumentNullException(nameof(inputString));
            }
            if(listString == null)
            {
                throw new AbandonedMutexException(nameof(listString));
            }
            foreach(var item in listString)
            {
                if(await CompareStringAsync(inputString, item))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


        public async Task AddRolesToUserAsync(User user, List<string> listRoles)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if(listRoles == null)
            {
                throw new ArgumentNullException(nameof(listRoles));
            }
            foreach (var role in listRoles.Distinct())
            {
                var roleOfUser = await GetRolesOfUserAsync(user);
                if(await IsStringInListAsync(role, roleOfUser.ToList()))
                {
                    throw new ArgumentException("Nguời dùng có quyền này rồi");
                }
                else
                {
                    var roleItem = await _context.Roles.SingleOrDefaultAsync(x => x.RoleCode.Equals(role));
                    if (roleItem == null)
                    {
                        throw new ArgumentNullException("Không tồn tại quyền này");
                    }
                    _context.Permissions.Add(new Permissions
                    {
                        RoleId = roleItem.Id,
                        UserId = user.Id,

                    });

                }
            }
            _context.SaveChanges();
        }
        //lay ra list quyen cua nguoi dung
        public async Task<IEnumerable<string>> GetRolesOfUserAsync(User user)
        {
            var roles = new  List<string>();
            var listRoles = _context.Permissions.Where(x => x.UserId == user.Id).AsQueryable();
            foreach(var item in listRoles.Distinct())
            {
                var role = _context.Roles.SingleOrDefault(x => x.Id == item.RoleId);
                roles.Add(role.RoleCode);
            }
            return roles.AsEnumerable();
        }
        public async Task<IEnumerable<Role>> GetRolesOfUser(User user)
        {
            var roles = new List<Role>();
            var listRoles = _context.Permissions.Where(x => x.UserId == user.Id).AsQueryable();
            foreach (var item in listRoles.Distinct())
            {
                var role = _context.Roles.SingleOrDefault(x => x.Id == item.RoleId);
                roles.Add(role);
            }

            return roles.AsEnumerable();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            return user;
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phoneNumber));
            return user;
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()));
            return user;
        }
        public async Task<IEnumerable<User>> GetUsersWithRoleManagerAsync()
        {
            // Lấy RoleId của quyền Leader
            var managerRole = await _context.Roles
                .Where(r => r.RoleCode == "Manager")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (managerRole == 0)  // Không tìm thấy vai trò Leader
            {
                return new List<User>();
            }

            // Lấy tất cả người dùng có RoleId = Leader
            var usersWithManagerRoleRole = await _context.Permissions
                .Where(p => p.RoleId == managerRole)
                .Select(p => p.UserId)
                .ToListAsync();

            // Trả về danh sách người dùng
            var users = await _context.Users
                .Where(u => usersWithManagerRoleRole.Contains(u.Id))
                .ToListAsync();

            return users;
        }
        public async Task<bool> IsUserInDesignTeamAsync(int userId)
        {
            // Kiểm tra xem người dùng có thuộc team "Design"
            var isInDesignTeam = await (from u in _context.Users
                                        join t in _context.Teams on u.TeamId equals t.Id
                                        where u.Id == userId && t.Name == "Designs"
                                        select u).AnyAsync();

            return isInDesignTeam;
        }
        public async Task<IEnumerable<Notification>> GetNotificationByUserId(int userId)
        {
            var user = await _context.Notifications.Where(x => x.UserId == userId).ToListAsync();
            return user;
        }

        public async Task<IEnumerable<Permissions>> GetPermissionsByUserId(int id)
        {
            var permissionId = await _context.Permissions.Where(x => x.UserId == id).ToListAsync();
            var response = permissionId.Select(x => new Permissions
            {
                Id = x.Id,
                RoleId = x.RoleId,
                UserId = x.UserId,
            });
            return response;
        }

        public async Task<IEnumerable<User>> GetAllUser(string? userName)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(u => u.UserName.Contains(userName));
            }

            return await query.ToListAsync();
        }

    }
}
