using System.Data.Common;
using Jwt.Identity.Data.IntialData;
using Jwt.Identity.Domain.Clients.Entity;
using Jwt.Identity.Domain.IdentityPolicy.Entity;
using Jwt.Identity.Domain.UseLoginPolicy.Entities;
using Jwt.Identity.Domain.User.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Jwt.Identity.Data.Context
{

    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
            //اطمینان از ساخت دیتا بیس جدید
          // this.Database.EnsureCreated();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<UserLogInOutLog> UserLogInOutLogs { get; set; }
        public DbSet<IdentitySettingPolicy> IdentitySettings { get; set; }
        public DbSet<UserLoginPolicyOptions> UserLoginPolicyOptions { get; set; }
        public DbSet<ApplicationUserPolicy> ApplicationUserPolicies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Client>()
                .HasIndex(b => b.ClientName)
                .IsUnique();
            builder.Entity<ApplicationUserPolicy>(x =>
            {
                x.HasKey(b => new { b.UserId, b.PolicyId });
                x.HasOne(d => d.ApplicationUser)
                    .WithOne(c => c.ApplicationUserPolicy)
                    .HasForeignKey<ApplicationUserPolicy>(u => u.UserId)
                    .HasPrincipalKey<ApplicationUser>(c => c.Id);
                x.HasOne(b=>b.UserLoginPolicyOption).
                    WithMany(c=>c.ApplicationUserPolicies)
                    .HasForeignKey(o => o.PolicyId)
                    .HasPrincipalKey(x => x.Id);
            });








            // builder.Entity<IdentitySetting>().HasData(new IdentitySetting());

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer();
        //    }
        //}
    }
    // زمان توسعه نرم افزار زمانی که ای اف نمی تواند سرویس را از هاست بگیرد
    public class DbContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=BoursYarIdentityDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new IdentityContext(optionsBuilder.Options);
        }
    }
}