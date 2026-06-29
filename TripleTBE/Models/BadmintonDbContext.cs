using Microsoft.EntityFrameworkCore;

namespace TripleTBE.Models;

public partial class BadmintonDbContext : DbContext
{
    public BadmintonDbContext()
    {
    }

    public BadmintonDbContext(DbContextOptions<BadmintonDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<News> News { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<ProductVariant> ProductVariants { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    // --- THÊM CÁC ĐỐI TƯỢNG ĐẶT SÂN & CHAT VÀO ĐÂY ---
    public virtual DbSet<Court> Courts { get; set; }
    public virtual DbSet<CourtReview> CourtReviews { get; set; }
    public virtual DbSet<CourtSubItem> CourtSubItems { get; set; }
    public virtual DbSet<CourtTimeSlot> CourtTimeSlots { get; set; }
    public virtual DbSet<CourtBooking> CourtBookings { get; set; }
    public virtual DbSet<CourtBookingDetail> CourtBookingDetails { get; set; }
    public virtual DbSet<CourtPayment> CourtPayments { get; set; }
    public virtual DbSet<ChatRoom> ChatRooms { get; set; }
    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning Move connection string to appsettings.json
        => optionsBuilder.UseSqlServer(
            "Server=.;Database=BadmintonDB;Trusted_Connection=True;TrustServerCertificate=True;"
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /* =========================================================
           BRAND
        ========================================================= */
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        /* =========================================================
           CATEGORY
        ========================================================= */
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
        });

        /* =========================================================
           USER
        ========================================================= */
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("Customer");
            entity.Property(e => e.Status).HasDefaultValue("Active");
        });

        /* =========================================================
           USER PROFILE
        ========================================================= */
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.User)
                .WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* =========================================================
           PRODUCT
        ========================================================= */
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.HasIndex(e => new { e.CategoryId, e.BrandId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Brand)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        /* =========================================================
           PRODUCT IMAGE
        ========================================================= */
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.HasOne(d => d.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* =========================================================
           PRODUCT VARIANT
        ========================================================= */
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.VariantId);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Stock).HasDefaultValue(0);

            entity.HasIndex(e => e.SKU).IsUnique();

            entity.HasOne(d => d.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* =========================================================
           CART
        ========================================================= */
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User)
                .WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* =========================================================
           CART ITEM
        ========================================================= */
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId);
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Cart)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Variant)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        /* =========================================================
           ORDER
        ========================================================= */
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderStatus).HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        /* =========================================================
           ORDER DETAIL
        ========================================================= */
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasComputedColumnSql("([Quantity]*[UnitPrice])", false);

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Variant)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        /* =========================================================
           PAYMENT
        ========================================================= */
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);

            entity.HasOne(d => d.Order)
                .WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* =========================================================
           REVIEW
        ========================================================= */
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        /* =========================================================
           NEWS
        ========================================================= */
        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User)
                .WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });


        /* =========================================================
           4. PHÂN HỆ: ĐẶT SÂN CẦU LÔNG (COURT BOOKINGS)
        ========================================================= */

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.CourtId);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue("Active");

            // Cấu hình cho thuộc tính mới thêm
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(2,1)")
                .HasDefaultValue(0m);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Owner)
                .WithMany(p => p.Courts)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        /* =========================================================
           COURT REVIEW
        ========================================================= */
        modelBuilder.Entity<CourtReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            // Cấu hình mối quan hệ với Court
            entity.HasOne(d => d.Court)
                .WithMany(p => p.CourtReviews)
                .HasForeignKey(d => d.CourtId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình mối quan hệ với User rõ ràng bằng Fluent API
            entity.HasOne(d => d.User)
                .WithMany(p => p.CourtReviews) // Trỏ tới User.CourtReviews
                .HasForeignKey(d => d.UserId)  // Khóa ngoại là UserId
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<CourtSubItem>(entity =>
        {
            entity.HasKey(e => e.SubCourtId);
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Court)
                .WithMany(p => p.CourtSubItems)
                .HasForeignKey(d => d.CourtId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CourtTimeSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.Court)
                .WithMany(p => p.CourtTimeSlots)
                .HasForeignKey(d => d.CourtId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CourtBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.BookingStatus).HasDefaultValue("Pending");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Unpaid");
            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User)
                .WithMany(p => p.CourtBookings) 
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CourtBookingDetail>(entity =>
        {
            entity.HasKey(e => e.BookingDetailId);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CheckInStatus).HasDefaultValue("NotAssigned");

            // Ràng buộc UNIQUE phòng tránh trùng lịch sân cầu lông
            entity.HasIndex(e => new { e.SubCourtId, e.PlayDate, e.StartTime })
                .IsUnique()
                .HasDatabaseName("UQ_SubCourt_Schedule");

            entity.HasOne(d => d.Booking)
                .WithMany(p => p.CourtBookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.SubCourt)
                .WithMany(p => p.CourtBookingDetails)
                .HasForeignKey(d => d.SubCourtId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CourtPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Booking)
                .WithMany(p => p.CourtPayments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /* =========================================================
           5. PHÂN HỆ: LIÊN LẠC TRỰC TUYẾN (CHATTING)
        ========================================================= */

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.RoomId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            // Ràng buộc UNIQUE phòng chat giữa Customer và Owner
            entity.HasIndex(e => new { e.CustomerId, e.OwnerId })
                .IsUnique()
                .HasDatabaseName("UQ_Customer_Owner_Room");

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.CustomerChatRooms) 
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Owner)
                .WithMany(p => p.OwnerChatRooms) 
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentAt).HasDefaultValueSql("(getdate())");

            // Tối ưu hóa Index theo Rooms & SentAt DESC
            entity.HasIndex(e => new { e.RoomId, e.SentAt });

            entity.HasOne(d => d.Room)
                .WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Sender)
                .WithMany(p => p.ChatMessages) 
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}