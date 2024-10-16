using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Infrastructure.DataContexts
{
    public class ApplicationDbContext : DbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected ApplicationDbContext()
        {
            
        }

        public virtual DbSet<Bill> Bills{ get; set; }
        public virtual DbSet<ConfirmEmail> ConfirmEmails { get; set; }
        public virtual DbSet<Customer> Customers{ get; set; }
        public virtual DbSet<CustomerFeedback> CustomerFeedbacks{ get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<Design> Designs { get; set; }
        public virtual DbSet<ImportCoupon> ImportCoupons{ get; set; }
        public virtual DbSet<KeyPerformanceIndicators> KeyPerformanceIndicators { get; set; }
        public virtual DbSet<Notification> Notifications{ get; set; }
        public virtual DbSet<Permissions> Permissions{ get; set; }
        public virtual DbSet<PrintJobs> PrintJobs { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens{ get; set; }
        public virtual DbSet<ResourceForPrintJob> ResourceForPrintJobs { get; set; }
        public virtual DbSet<ResourceProperty> ResourceProperties { get; set; }
        public virtual DbSet<ResourcePropertyDetail> ResourcePropertyDetails { get; set; }
        public virtual DbSet<Resources> Resources { get; set; }
        public virtual DbSet<Role> Roles { get; set; } 
        public virtual DbSet<ShippingMethod> ShippingMethods { get; set; }
        public virtual DbSet<Team> Teams{ get; set; }
        public virtual DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table: User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Team)
                .WithMany(t => t.Users) // Một Team có nhiều Users
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.Restrict); // Điều này để tránh vòng lặp xóa khi Manager bị xóa

            // Table: Team
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Manager) // Một Team có một Manager
                .WithMany(t => t.Teams) // Một User có thể là Manager của nhiều Team (nhưng không cần thuộc tính đối ngược)
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh vòng lặp xóa giữa User và Team
            // Table: Permissions
            modelBuilder.Entity<Permissions>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Permissions>()
                .HasOne(p => p.Role)
                .WithMany()
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: ConfirmEmail
            modelBuilder.Entity<ConfirmEmail>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: Project
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Customer)
                .WithMany()
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: KeyPerformanceIndicators
            modelBuilder.Entity<KeyPerformanceIndicators>()
                .HasOne(k => k.Employee)
                .WithMany()
                .HasForeignKey(k => k.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: CustomerFeedback
            modelBuilder.Entity<CustomerFeedback>()
                .HasOne(cf => cf.Project)
                .WithMany()
                .HasForeignKey(cf => cf.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CustomerFeedback>()
                .HasOne(cf => cf.Customer)
                .WithMany()
                .HasForeignKey(cf => cf.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CustomerFeedback>()
                .HasOne(cf => cf.UserFeedback)
                .WithMany()
                .HasForeignKey(cf => cf.UserFeedbackId)
                .OnDelete(DeleteBehavior.NoAction);

            // Table: Resources
            modelBuilder.Entity<ResourceProperty>()
                .HasOne(rp => rp.Resource)
                .WithMany()
                .HasForeignKey(rp => rp.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResourcePropertyDetail>()
                .HasOne(rpd => rpd.ResourceProperty)
                .WithMany()
                .HasForeignKey(rpd => rpd.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: Delivery
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Customer)
                .WithMany()
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.ShippingMethod)
                .WithMany()
                .HasForeignKey(d => d.ShippingMethodId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Project)
                .WithMany()
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            // Table: Design
            modelBuilder.Entity<Design>()
                .HasOne(d => d.Project)
                .WithMany()
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Design>()
                .HasOne(d => d.Designer)
                .WithMany()
                .HasForeignKey(d => d.DesignerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Design>()
                .HasOne(d => d.Approver)
                .WithMany()
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.NoAction);

            // Table: PrintJobs
            modelBuilder.Entity<PrintJobs>()
                .HasOne(pj => pj.Design)
                .WithMany()
                .HasForeignKey(pj => pj.DesignId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: Bill
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Project)
                .WithMany()
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Employee)
                .WithMany()
                .HasForeignKey(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Table: ResourceForPrintJob
            modelBuilder.Entity<ResourceForPrintJob>()
                .HasOne(rfpj => rfpj.ResourcePropertyDetail)
                .WithMany()
                .HasForeignKey(rfpj => rfpj.ResourcePropertyDetailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResourceForPrintJob>()
                .HasOne(rfpj => rfpj.PrintJob)
                .WithMany()
                .HasForeignKey(rfpj => rfpj.PrintJobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: ImportCoupon
            modelBuilder.Entity<ImportCoupon>()
                .HasOne(ic => ic.ResourcePropertyDetail)
                .WithMany()
                .HasForeignKey(ic => ic.ResourcePropertyDetailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ImportCoupon>()
                .HasOne(ic => ic.Employee)
                .WithMany()
                .HasForeignKey(ic => ic.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table: Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }


        public async Task<int> CommitChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
