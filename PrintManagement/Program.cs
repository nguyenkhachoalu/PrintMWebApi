using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrintManagement.Application.Handle.HandleEmail;
using PrintManagement.Application.ImplementServices;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Constants;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using PrintManagement.Infrastructure.DataContexts;
using PrintManagement.Infrastructure.ImplementRepositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString
    (Constant.AppSettingKeys.DEFAULT_CONNECTION)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:8080")  // Thay thế bằng URL của frontend
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();  // Cho phép gửi cookies
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});
builder.Services.AddAuthorization(options =>
{
    // Chỉ Admin mới có quyền thực hiện CRUD phòng ban, thay đổi trưởng phòng, chuyển phòng ban cho nhân viên
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Chỉ nhân viên thuộc phòng ban Sales mới có quyền tạo project
    options.AddPolicy("EmployeeAndSales", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Employee") &&
            context.User.HasClaim(c => c.Type == "Team" && c.Value == "Sales")));

    // Chỉ người dùng đã xác thực mới có quyền xem thông tin người dùng
    options.AddPolicy("AuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
});


builder.Services.AddScoped<IDbContext, ApplicationDbContext>();
//service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITeamService, TeamServices>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IDesignService, DesignService>();
builder.Services.AddScoped<IPrintJobService, PrintJobService>();
builder.Services.AddScoped<IResourcePropertyServices, ResourcePropertyService>();
builder.Services.AddScoped<IResourcesService, ResourcesService>();

//repository
builder.Services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
builder.Services.AddScoped<IBaseRepository<ConfirmEmail>, BaseRepository<ConfirmEmail>>();
builder.Services.AddScoped<IBaseRepository<Permissions>, BaseRepository<Permissions>>();
builder.Services.AddScoped<IBaseRepository<RefreshToken>, BaseRepository<RefreshToken>>();
builder.Services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
builder.Services.AddScoped<IBaseRepository<Team>, BaseRepository<Team>>();
builder.Services.AddScoped<IBaseRepository<Project>, BaseRepository<Project>>();
builder.Services.AddScoped<IBaseRepository<Notification>, BaseRepository<Notification>>();
builder.Services.AddScoped<IBaseRepository<Design>, BaseRepository<Design>>();
builder.Services.AddScoped<IBaseRepository<Resources>, BaseRepository<Resources>>();
builder.Services.AddScoped<IBaseRepository<ResourceForPrintJob>, BaseRepository<ResourceForPrintJob>>();
builder.Services.AddScoped<IBaseRepository<ResourceProperty>, BaseRepository<ResourceProperty>>();
builder.Services.AddScoped<IBaseRepository<PrintJobs>, BaseRepository<PrintJobs>>();
builder.Services.AddScoped<IBaseRepository<ResourceForPrintJob>, BaseRepository<ResourceForPrintJob>>();
builder.Services.AddScoped<IBaseRepository<Customer>, BaseRepository<Customer>>();
builder.Services.AddScoped<IBaseRepository<ShippingMethod>, BaseRepository<ShippingMethod>>();
builder.Services.AddScoped<IBaseRepository<Delivery>, BaseRepository<Delivery>>();
builder.Services.AddScoped<IBaseRepository<ResourcePropertyDetail>, BaseRepository<ResourcePropertyDetail>>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDesignRepository, DesignRepository>();
builder.Services.AddScoped<IResourcesRepository, ResourcesRepository>();
builder.Services.AddScoped<IPrintJobsRepository, PrintJobsRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IResourcePropertyRepository, ResourcePropertyRepository>();
//converter
builder.Services.AddScoped<UserConverter>();
builder.Services.AddScoped<DeliveryConveter>();
builder.Services.AddScoped<TeamConverter>();
builder.Services.AddScoped<DesignConveter>();
builder.Services.AddScoped<ProjectConverter>();
builder.Services.AddScoped<ResourcesConveter>();
builder.Services.AddScoped<ResourcePropertyConveter>();
builder.Services.AddScoped<ResourceForPrintJobConveter>();
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "PrintManagement", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nhập token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Upload", "Avatar")), // Thư mục chứa các tệp ảnh avatar
    RequestPath = "/images/avatars"  // Đường dẫn yêu cầu để truy cập các ảnh này
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Upload", "Project")),
    RequestPath = "/images/projects"  // Đường dẫn cho ảnh project
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Upload", "ResourcePropertyDetail")),
    RequestPath = "/images/resourceDetails"  // Đường dẫn cho ảnh project
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Upload", "Design")),
    RequestPath = "/filePaths/designs"  // Đường dẫn cho ảnh Design
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Upload", "Resources")),
    RequestPath = "/images/resources"  // Đường dẫn cho ảnh Resources
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigins");  // Thay thế "AllowAllOrigins" bằng "AllowSpecificOrigins"


app.MapControllers();

app.Run();
