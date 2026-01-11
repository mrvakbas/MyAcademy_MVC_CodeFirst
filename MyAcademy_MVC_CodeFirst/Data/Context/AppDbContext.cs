using Microsoft.AspNet.Identity.EntityFramework;
using MyAcademy_MVC_CodeFirst.Data.Entities;
using System.Data.Entity;

namespace MyAcademy_MVC_CodeFirst.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public AppDbContext() : base("name=AppDbContext")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Packages> Packages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .ToTable("Users")
                .Property(p => p.Name).HasMaxLength(50).IsRequired();

            modelBuilder.Entity<AppRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");

            modelBuilder.Entity<Policy>()
                    .HasRequired(p => p.Category)
                    .WithMany(c => c.Policies)
                    .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Policy>()
                .HasRequired(p => p.AppUser)
                .WithMany(u => u.Policies)
                .HasForeignKey(p => p.AppUserId);
        }
    }
}