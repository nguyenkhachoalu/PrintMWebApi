using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using PrintManagement.Application.Handle.HandleEmail;
using PrintManagement.Application.Handle.HandleFile;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.AuthRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using PrintManagement.Domain.Validations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using BcryptNet = BCrypt.Net.BCrypt;
namespace PrintManagement.Application.ImplementServices
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        private readonly IBaseRepository<Permissions> _basePermissionRepository;
        private readonly IBaseRepository<RefreshToken> _baseRefreshTokenRepository;
        private readonly IBaseRepository<Role> _baseRoleRepository;
        private readonly UserConverter _userConverter;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IBaseRepository<User> baseUserRepository, IBaseRepository<ConfirmEmail> baseConfirmEmailRepository, IBaseRepository<Permissions> basePermissionRepository, IBaseRepository<RefreshToken> baseRefreshTokenRepository, IBaseRepository<Role> baseRoleRepository, UserConverter userConverter, IConfiguration configuration, IUserRepository userRepository, IEmailService emailService, IRefreshTokenRepository refreshTokenRepository)
        {
            _baseUserRepository = baseUserRepository;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
            _basePermissionRepository = basePermissionRepository;
            _baseRefreshTokenRepository = baseRefreshTokenRepository;
            _baseRoleRepository = baseRoleRepository;
            _userConverter = userConverter;
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> ConfirmRegisterAccount(string confirmCode)
        {
            try
            {
                var code = await _baseConfirmEmailRepository.GetAsync(x => x.ConfirmCode.Equals(confirmCode));
                if (code == null)
                {
                    return "Mã xác nhận không hợp lệ";
                }
                if (code.IsConfirm)
                {
                    return "Mã đã được sử dụng";
                }
                var user = await _baseUserRepository.GetAsync(x => x.Id == code.UserId);
                if (code.ExpiryTime < DateTime.Now)
                {
                    return "Mã xác nhận đã hết hạn";
                }
                user.IsActive = true;
                code.IsConfirm = true;
                await _baseUserRepository.UpdateAsync(user);
                await _baseConfirmEmailRepository.UpdateAsync(code);
                return "Xác nhận đăng ký thành công";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<ResponseObject<DataResponseUser>> Register(Request_Register request)
        {
            try
            {
                #region validCheck
                if (request.UserName == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài khoản không được để trống",
                        Data = null
                    };
                }
                if (request.Password == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Mật khẩu không được để trống",
                        Data = null
                    };
                }
                if (request.FullName == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tên người không được để trống",
                        Data = null
                    };
                }
                if (request.DateOfBirth == DateTime.MinValue)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Ngày sinh không được để trống",
                        Data = null
                    };
                }
                if (request.Email == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Email không được đê trống",
                        Data = null
                    };
                }
                if (!ValidateInput.IsValidEmail(request.Email))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Định dạng email không hợp lệ",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserByEmail(request.Email) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Email này đã tồn tại trên hệ thống",
                        Data = null
                    };
                }
                if (request.PhoneNumber == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Số điện thoại không được để trống",
                        Data = null
                    };
                }
                if (!ValidateInput.isValidPhoneNumber(request.PhoneNumber))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Định dạng số điện thoại không hợp lệ",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserByPhoneNumber(request.PhoneNumber) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Số điện thoại này đã tồn tại trên hệ thống",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserByUserName(request.UserName) != null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Tài khoản đã tồn tại",
                        Data = null
                    };
                }

                #endregion

                var user = new User
                {
                    UserName = request.UserName,
                    Password = BcryptNet.HashPassword(request.Password),
                    FullName = request.FullName,
                    DateOfBirth = request.DateOfBirth,
                    Avatar = await HandleUploadFile.WirteFile(request.Avatar),
                    Email = request.Email,
                    CreateTime = DateTime.Now,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = false,
                };
                await _baseUserRepository.CreateAsync(user);
                await _userRepository.AddRolesToUserAsync(user, new List<string> { "Employee" });
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    UserId = user.Id,
                    ConfirmCode = GenerateCodeActive(),
                    ExpiryTime = DateTime.Now.AddMinutes(3),
                    CreateTime = DateTime.Now,
                    IsConfirm = false,
                };
                await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
                var emailContent = _emailService.GenerateConfirmationCodeEmail(confirmEmail.ConfirmCode);
                var message = new EmailMessage(new string[] { request.Email }, $"Mã xác nhận của {user.FullName} ", emailContent);
                var responseMessage = _emailService.SendEmail(message);
                var responseUser = _userConverter.EntitytoDTO(user);
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đăng ký thành công! Vui lòng Nhận mã xác nhận tại email",
                    Data = responseUser
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }
        public async Task<ResponseObject<DataResponseLogin>> Login(Request_Login request)
        {
            try
            {
                var user = await _baseUserRepository.GetAsync(x => x.UserName.Equals(request.UserName));
                if (user == null)
                {
                    return new ResponseObject<DataResponseLogin>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Sai tên tài khoản",
                        Data = null,
                    };
                }
                if (!user.IsActive)
                {
                    return new ResponseObject<DataResponseLogin>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Tài khoản bạn đang bị vô hiệu hóa",
                        Data = null,
                    };
                }

                bool checkPass = BcryptNet.Verify(request.Password, user.Password);
                if (!checkPass)
                {
                    return new ResponseObject<DataResponseLogin>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Mật khẩu không chính xác",
                        Data = null,
                    };
                }
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đăng nhập thành công",
                    Data = new DataResponseLogin
                    {
                        AccessToken = GetJwtTokenAsync(user).Result.Data.AccessToken,
                        RefreshToken = GetJwtTokenAsync(user).Result.Data.RefreshToken,
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseLogin>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = null,
                };
            }
        }
        public async Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user)
        {
            var permissions = await _basePermissionRepository.GetAllAsync(x => x.UserId == user.Id);
            var roles = await _baseRoleRepository.GetAllAsync();
            var authClaims = new List<Claim>
            {
                    new Claim("Id",user.Id.ToString()),
                    new Claim("UserName", user.UserName.ToString()),
                    new Claim("Email", user.Email.ToString()),
                    new Claim("PhoneNumber", user.PhoneNumber.ToString()),
            };
            foreach (var permission in permissions)
            {
                foreach (var role in roles)
                {
                    if(role.Id == permission.RoleId)
                    {
                        authClaims.Add(new Claim("Permission", role.RoleName));

                    }
                }
            }
            var userRole = await _userRepository.GetRolesOfUserAsync(user);
            foreach (var item in userRole)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, item));
            }
            var jwtToken = GetToken(authClaims);
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);

            RefreshToken rf = new RefreshToken
            {
                ExpiryTime = DateTime.Now.AddHours(refreshTokenValidity),
                UserId = user.Id,
                Token = refreshToken,
                CreateTime = DateTime.Now,
                IsActive = true,
            };
            rf = await _baseRefreshTokenRepository.CreateAsync(rf);
            return new ResponseObject<DataResponseLogin>
            {
                Status = StatusCodes.Status201Created,
                Message = "Tạo token thành công",
                Data = new DataResponseLogin
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    RefreshToken = refreshToken,
                }
            };
        }
        public Task<(string accessToken, string refreshToken)> RefreshTokenAsync(Request_Token request)
        {
            throw new NotImplementedException();
        }
        public async Task<ResponseObject<DataResponseUser>> ChangePassword(int userId, Request_ChangePassword request)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(userId);
                bool checkPass = BcryptNet.Verify(request.OldPassword, user.Password);
                if (!checkPass)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Mật khẩu không chính xác",
                        Data = null
                    };
                }
                if (!request.NewPassword.Equals(request.ConfirmPassword))
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Mật khẩu xác nhận không khớp",
                        Data = null,
                    };
                }
                user.Password = BcryptNet.HashPassword(request.NewPassword);
                user.UpdateTime = DateTime.Now;
                await _baseUserRepository.UpdateAsync(user);
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Mật khẩu thay đổi thành công",
                    Data = _userConverter.EntitytoDTO(user)
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = null,
                };
            }

        }
        public async Task<string> ForgotPassword(string userName)
        {
            var user = await _userRepository.GetUserByUserName(userName);
            if(user == null)
            {
                return "Tài khoản không tồn tại trong hệ thống";
            }
            ConfirmEmail confirmEmail = new ConfirmEmail
            {
                UserId = user.Id,
                ConfirmCode = GenerateCodeActive(),
                ExpiryTime = DateTime.Now.AddMinutes(3),
                CreateTime = DateTime.Now,
                IsConfirm = false,
            };
            await _baseConfirmEmailRepository.CreateAsync(confirmEmail);
            var emailContent = _emailService.GenerateConfirmationCodeEmail(confirmEmail.ConfirmCode);
            var message = new EmailMessage(new string[] { user.Email }, $"Mã xác nhận của {user.FullName} ", emailContent);
            var responseMessage = _emailService.SendEmail(message);
           
            return "Hệ thống đã gửi mã xác minh tới email của bạn, thời hạn 3 phút";
        }
        public async Task<string> ConfirmForgotPassword(string confirmCode, Request_ForgotPassword request)
        {
           var code = await _baseConfirmEmailRepository.GetAsync(x => x.ConfirmCode.Equals(confirmCode));
            if (code == null)
            {
                return "Mã xác nhận không hợp lệ";
            }
            if (code.IsConfirm)
            {
                return "Mã đã được sử dụng";
            }
            if (code.ExpiryTime < DateTime.Now)
            {
                return "Mã xác nhận đã hết hạn";
            }
            var user = await _baseUserRepository.GetAsync(x => x.Id == code.UserId);
            user.Password = BcryptNet.HashPassword(request.NewPassword);
            code.IsConfirm = true;
            await _baseUserRepository.UpdateAsync(user);
            await _baseConfirmEmailRepository.UpdateAsync(code);
            return "Đã đổi mật khẩu thành công";
        }
        public async Task<ResponseObject<string>> LogoutAsync(string token)
        {
            try
            {
                var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
                if (refreshToken == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Token không hợp lệ hoặc không tồn tại.",
                        Data = null,
                    };
                }
                if (!refreshToken.IsActive)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Token đã hết hạn hoặc đã bị hủy.",
                        Data = null,
                    };
                }
                refreshToken.IsActive = false;
                await _baseRefreshTokenRepository.UpdateAsync(refreshToken);
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đăng xuất thành công",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = null
                };
            }
        }
        #region Private Methods
        private string GenerateCodeActive()
        {
            Random random = new Random();
            int code = random.Next(0, 100000); // Tạo số ngẫu nhiên từ 0 đến 99999
            return code.ToString("D5"); // Chuyển đổi thành chuỗi có độ dài 5, thêm '0' ở đầu nếu cần
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])),
                ValidateLifetime = false // Quan trọng: Không validate thời hạn của token
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInHours"], out int tokenValidityInHours);
            var expiration = DateTime.Now.AddHours(tokenValidityInHours);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expiration,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var jwtToken = GetToken(claims.ToList());
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

 

        #endregion
    }
}
