using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class UserConverter
    {
        public DataResponseUser EntitytoDTO(User user)
        {
            return new DataResponseUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Avatar = user.Avatar,
                Email = user.Email,
                CreateTime = user.CreateTime,
                UpdateTime = user.UpdateTime,
                PhoneNumber = user.PhoneNumber,
                TeamId = user.TeamId,
                IsActive = user.IsActive,
            };
        }
    }
}
