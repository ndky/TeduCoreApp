using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using TeduCoreApp.Data.Entities;
using TeduCoreApp.Data.Interfaces;

namespace TeduCoreApp.Data.EF
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Language> Languages { set; get; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<Function> Functions { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<Announcement> Announcements { set; get; }
        public DbSet<AnnouncementUser> AnnouncementUsers { set; get; }

        public DbSet<Blog> Bills { set; get; }
        public DbSet<BillDetail> BillDetails { set; get; }
        public DbSet<Blog> Blogs { set; get; }
        public DbSet<BlogTag> BlogTags { set; get; }
        public DbSet<Color> Colors { set; get; }
        public DbSet<Contact> Contacts { set; get; }
        public DbSet<Feedback> Feedbacks { set; get; }
        public DbSet<Footer> Footers { set; get; }
        public DbSet<Page> Pages { set; get; }
        public DbSet<Product> Products { set; get; }
        public DbSet<ProductCategory> ProductCategories { set; get; }
        public DbSet<ProductImage> ProductImages { set; get; }
        public DbSet<ProductQuantity> ProductQuantities { set; get; }
        public DbSet<ProductTag> ProductTags { set; get; }

        public DbSet<Size> Sizes { set; get; }
        public DbSet<Slide> Slides { set; get; }

        public DbSet<Tag> Tags { set; get; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<WholePrice> WholePrices { get; set; }

        public DbSet<AdvertistmentPage> AdvertistmentPages { get; set; }
        public DbSet<Advertistment> Advertistments { get; set; }
        public DbSet<AdvertistmentPosition> AdvertistmentPositions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims")
                .HasKey(x => x.Id);

            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);

            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles")
                .HasKey(x => new { x.RoleId, x.UserId });

            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens")
               .HasKey(x => new { x.UserId });
            //Cấu hình Announcement là varchar(128)
            builder.Entity<Announcement>(entity=> {
                entity.Property(e => e.Id).HasColumnType("varchar(128)");
            });
            //Cấu hình Tag với Id là varchar(50)
            builder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(50).IsRequired().HasColumnType("varchar(50)");

            });
            //Cấu hình advertistmentPosition với id có chiều dài tối đa 50
            builder.Entity<AdvertistmentPosition>(entity => {
                entity.Property(e => e.Id).HasMaxLength(20).IsRequired();
            });
            //Cấu hình Blogtag với TagId là varchar(50)
            builder.Entity<BlogTag>(entity => {
                entity.Property(e => e.TagId).HasMaxLength(50).IsRequired().HasColumnType("varchar(50)");
            });
            //Cấu hình contact
            builder.Entity<Contact>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(255).IsRequired();
            });
            //Cấu hình footer
            builder.Entity<Footer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(255).HasColumnType("varchar(255)").IsRequired();
            });
            //Cấu hình function
            builder.Entity<Function>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(c => c.Id).IsRequired().HasColumnType("varchar(128)");
            });
            //Cấu hình page
            builder.Entity<Page>(entity => {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasMaxLength(255).IsRequired();
            });
            //Cấu hình ProductTag
            builder.Entity<ProductTag>(entity => {
                entity.Property(c => c.TagId).HasMaxLength(50).IsRequired().HasColumnType("varchar(50)");
            });
            //Cấu hình SystemConfig
            builder.Entity<SystemConfig>(entity => {
                entity.Property(c => c.Id).HasMaxLength(255).IsRequired();
            });
            //base.OnModelCreating(builder);
        }
        public override int SaveChanges()
        {
            var modified = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);
            foreach (EntityEntry item in modified)
            {
                var changedOrAdded = item.Entity as IDateTracking;
                if (changedOrAdded != null)
                {
                    if (item.State == EntityState.Modified)
                        changedOrAdded.DateModified = DateTime.Now;
                    changedOrAdded.DateCreated = DateTime.Now;
                }
            }
            return base.SaveChanges();
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new  DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new AppDbContext(builder.Options);
        }
    }
}