using Millionlights.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Millionlights.Models
{
    public class MillionlightsContext : DbContext
    {
        public MillionlightsContext() : base("name=MillionLight")
        {
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseContent> CourseContent { get; set; }
        public DbSet<CATS> Cats { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerType> PartnerType { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UsersDetails { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserInRole> UserInRoles { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<ProductCode> ProductCode { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<ItemsOrdered> ItemsOrders { get; set; }
        public DbSet<UsersCourses> UsersCourses { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<CourseLevel> CourseLevels { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseDelivery> CourseDeliveris { get; set; }
        public DbSet<CourseLanguage> CourseLanguages { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<CourseAvailability> CourseAvailability { get; set; }
        public DbSet<VoucherCode> VoucherCode { get; set; }
        public DbSet<VoucherCourses> VoucherCourses { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<BenifitType> BenifitTypes { get; set; }
        public DbSet<CouponStatus> CouponStatus { get; set; }
        public DbSet<CouponCourses> CouponCourses { get; set; }
        public DbSet<TmpUser> TmpUsers { get; set; }
        public DbSet<DuplicateRecords> DuplicateRecords { get; set; }

        public DbSet<AuthTokens> AuthTokens { get; set; }

        public DbSet<NotificationStatus> NotificationStatus { get; set; }

        public DbSet<UserNotitification> UserNotitifications { get; set; }

        public DbSet<HomePageConfiguration> HomePageConfigurations { get; set; }
        public DbSet<UserCoupons> UserCoupons { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<UsersTable> UsersTable { get; set; }
        public DbSet<UsersCertificate> UsersCertificate { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequest { get; set; }
        public DbSet<CertificateEvidenceLkp> CertificateEvidenceLkp { get; set; }
        public DbSet<UserCertificateEvidenceDetails> UserCertificateEvidenceDetails { get; set; }
        public DbSet<PressContents> PressContents { get; set; }
        public DbSet<UserWallets> UserWallets { get; set; }
        public DbSet<ReferralCodes> ReferralCodes { get; set; }
        //public DbSet<ImportUserConfigurations> ImportUserConfigurations { get; set; }
        public DbSet<UsersCourseRatings> UsersCourseRatings { get; set; }
        public DbSet<Career> Careers { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}