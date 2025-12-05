using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventBookingWeb.Models.DomainModels
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public new DbSet<DBUser> Users { get; set; }
        public DbSet<DBNews> News { get; set; }
        public DbSet<DBEvent> Events { get; set; }
        public DbSet<DBCategoryEvent> CategoryEvents { get; set; }
        public DbSet<DBBooking> Bookings { get; set; }
        public DbSet<DBBanner> Banners { get; set; }
        public DbSet<DBTicket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DBUser>().HasKey(u => u.UserId); // Khóa chính
            modelBuilder.Entity<DBUser>().Property(u => u.UserId).ValueGeneratedOnAdd();
            modelBuilder.Entity<DBUser>().Property(u => u.Role).IsRequired();
            modelBuilder.Entity<DBUser>().Property(u => u.Email).IsRequired();
            modelBuilder.Entity<DBUser>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<DBUser>().Property(u => u.PasswordHash).IsRequired();
            modelBuilder.Entity<DBUser>().Property(u => u.UserStatus).IsRequired();

            modelBuilder.Entity<DBBooking>().HasKey(b => b.BookingId); // Khóa chính
            modelBuilder.Entity<DBBooking>().Property(b => b.EventId).IsRequired();
            modelBuilder.Entity<DBBooking>().Property(b => b.UserId).IsRequired();
            modelBuilder.Entity<DBBooking>().Property(b => b.Quantity).IsRequired();
            modelBuilder.Entity<DBBooking>().Property(b => b.TotalAmount).HasPrecision(18, 2).IsRequired();
            modelBuilder.Entity<DBBooking>().Property(b => b.PaymentStatus).IsRequired();
            modelBuilder.Entity<DBBooking>().Property(b => b.PaymentMethod).IsRequired();


            modelBuilder.Entity<DBCategoryEvent>().HasKey(c => c.CategoryEventId);
            modelBuilder.Entity<DBCategoryEvent>().Property(c => c.CategoryEventName).IsRequired();


            modelBuilder.Entity<DBEvent>().HasKey(e => e.EventId);
            modelBuilder.Entity<DBEvent>().Property(e => e.EventCategoryId).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.Title).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.EventContentHtml).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.Location).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.StartDate).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.EndDate).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.TotalSeats).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.AvailableSeats).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.TicketPrice).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.ImageUrl).IsRequired();
            modelBuilder.Entity<DBEvent>().Property(e => e.EventStatus).IsRequired();


            modelBuilder.Entity<DBNews>().HasKey(e => e.NewsId);
            modelBuilder.Entity<DBNews>().Property(b => b.EventId).IsRequired();
            modelBuilder.Entity<DBNews>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<DBNews>().Property(b => b.NewsContentHtml).IsRequired();
            modelBuilder.Entity<DBNews>().Property(b => b.ImageUrl).IsRequired();


            modelBuilder.Entity<DBTicket>().HasKey(e => e.TicketId);
            modelBuilder.Entity<DBTicket>().Property(b => b.BookingID).IsRequired();
            modelBuilder.Entity<DBTicket>().Property(b => b.UserID).IsRequired();
            modelBuilder.Entity<DBTicket>().Property(b => b.TicketCode).IsRequired();
            modelBuilder.Entity<DBTicket>().Property(b => b.QRCodeData).IsRequired();


            modelBuilder.Entity<DBBanner>().HasKey(b => b.BannerID);


            modelBuilder.Entity<DBBooking>()
                .HasOne(b => b.User)            // 1 Booking thuộc 1 User
                .WithMany(u => u.Bookings)      // 1 User có nhiều Booking
                .HasForeignKey(b => b.UserId)   // Khóa ngoại
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa User, xóa luôn Booking nếu cần

            modelBuilder.Entity<DBTicket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DBTicket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DBEvent>()
                .HasMany(e => e.CategoryEvents)
                .WithMany(c => c.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "DBEventCategoryEvent", // tên bảng trung gian
                    j => j.HasOne<DBCategoryEvent>()
                        .WithMany()
                        .HasForeignKey("CategoryEventId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<DBEvent>()
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("EventId", "CategoryEventId"); // khóa chính kép
                        j.ToTable("DBEventCategoryEvent");
                    });

            modelBuilder.Entity<DBNews>()
                .HasOne(n => n.Event)
                .WithMany(e => e.NewsList)
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
