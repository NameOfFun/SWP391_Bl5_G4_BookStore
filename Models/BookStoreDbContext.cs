using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class BookStoreDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public BookStoreDbContext()
    {
    }

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookTag> BookTags { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentRefund> PaymentRefunds { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    public virtual DbSet<BookRating> BookRatings { get; set; }

    public virtual DbSet<CustomerRequest> CustomerRequests { get; set; }

    public virtual DbSet<About> About { get; set; }

    public virtual DbSet<Policy> Policy { get; set; }

    public virtual DbSet<HomeSlider> HomeSliders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Author__70DAFC34FFED1BE6");

            entity.ToTable("Author");

            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Book__3DE0C20785DAE3CC");

            entity.ToTable("Book");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PromotionalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PromotionalStartsAt).HasColumnType("datetime");
            entity.Property(e => e.PromotionalEndsAt).HasColumnType("datetime");
            entity.Property(e => e.PricingNote).HasMaxLength(500);
            entity.Property(e => e.LastPricingChangeAt).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(e => e.LastPricingChangeBy)
                .WithMany()
                .HasForeignKey(e => e.LastPricingChangeByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Book__AuthorId__2A4B4B5E");

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Book__CategoryId__29572725");

            entity.HasMany(d => d.Tags).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookTagMapping",
                    r => r.HasOne<BookTag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookTagMa__TagId__300424B4"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookTagMa__BookI__2F10007B"),
                    j =>
                    {
                        j.HasKey("BookId", "TagId").HasName("PK__BookTagM__EBB70D9D3658287C");
                        j.ToTable("BookTagMapping");
                    });

            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<BookTag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__BookTag__657CF9AC0D8A39E4");

            entity.ToTable("BookTag");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B7D56E1D0");

            entity.ToTable("Category");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("Cart");
            entity.HasKey(e => e.CartId);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("CartItem");
            entity.HasKey(e => e.CartItemId);
            entity.HasIndex(e => new { e.CartId, e.BookId }).IsUnique();
            entity.HasOne(e => e.Cart).WithMany(c => c.Items).HasForeignKey(e => e.CartId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Book).WithMany(b => b.CartItems).HasForeignKey(e => e.BookId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.ToTable("Voucher");
            entity.HasKey(e => e.VoucherId);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DiscountType).HasMaxLength(1).IsFixedLength();
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.HasOne(e => e.LastManagedBy)
                .WithMany()
                .HasForeignKey(e => e.LastManagedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderDate).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ShippingName).HasMaxLength(200);
            entity.Property(e => e.ShippingPhone).HasMaxLength(30);
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.DeliveryNote).HasMaxLength(500);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Shipper).WithMany().HasForeignKey(e => e.ShipperId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Voucher).WithMany(v => v.Orders).HasForeignKey(e => e.VoucherId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");
            entity.HasKey(e => e.OrderDetailId);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Order).WithMany(o => o.Details).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Book).WithMany(b => b.OrderDetails).HasForeignKey(e => e.BookId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.Amount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Currency).HasMaxLength(3).IsFixedLength();
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.ProviderTransactionId).HasMaxLength(128);
            entity.Property(e => e.ProviderOrderRef).HasMaxLength(128);
            entity.Property(e => e.ClientIp).HasMaxLength(45);
            entity.Property(e => e.FailureCode).HasMaxLength(50);
            entity.Property(e => e.FailureMessage).HasMaxLength(500);
            entity.Property(e => e.GatewayRawResponse).HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.PaidAt).HasColumnType("datetime");
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProviderTransactionId)
                .IsUnique()
                .HasFilter("[ProviderTransactionId] IS NOT NULL");
            entity.HasOne(e => e.Order).WithMany(o => o.Payments).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PaymentRefund>(entity =>
        {
            entity.ToTable("PaymentRefund");
            entity.HasKey(e => e.PaymentRefundId);
            entity.Property(e => e.Amount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RefundedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ExternalRefundId).HasMaxLength(128);
            entity.HasIndex(e => e.PaymentId);
            entity.HasOne(e => e.Payment).WithMany(p => p.Refunds).HasForeignKey(e => e.PaymentId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.ToTable("Wishlist");
            entity.HasKey(e => new { e.UserId, e.BookId });
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Book).WithMany(b => b.Wishlists).HasForeignKey(e => e.BookId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookRating>(entity =>
        {
            entity.ToTable("BookRating");
            entity.HasKey(e => e.BookRatingId);
            entity.HasIndex(e => new { e.UserId, e.BookId }).IsUnique();
            entity.Property(e => e.Comment).HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.IsHidden).HasDefaultValue(false);
            entity.Property(e => e.HiddenAt).HasColumnType("datetime");
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Book).WithMany(b => b.BookRatings).HasForeignKey(e => e.BookId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.HiddenBy)
                .WithMany()
                .HasForeignKey(e => e.HiddenByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<CustomerRequest>(entity =>
        {
            entity.ToTable("CustomerRequest");
            entity.HasKey(e => e.CustomerRequestId);
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.Body).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ContactPhone).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.ResolutionNotes).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ResolvedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.HasIndex(e => e.OrderId);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.AssignedToUserId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.ResolvedBy)
                .WithMany()
                .HasForeignKey(e => e.ResolvedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Order)
                .WithMany(o => o.CustomerRequests)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<About>(entity =>
        {
            entity.ToTable("About", t =>
            {
                t.HasCheckConstraint("CK_About_Title_NonEmpty", "LEN(LTRIM(RTRIM([Title]))) > 0");
            });
            entity.HasKey(e => e.AboutId);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.ContentHtml).HasColumnType("nvarchar(max)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.ToTable("Policy", t =>
            {
                t.HasCheckConstraint("CK_Policy_Title_NonEmpty", "LEN(LTRIM(RTRIM([Title]))) > 0");
            });
            entity.HasKey(e => e.PolicyId);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.ContentHtml).HasColumnType("nvarchar(max)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<HomeSlider>(entity =>
        {
            entity.ToTable("HomeSlider");
            entity.HasKey(e => e.HomeSliderId);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Caption).HasMaxLength(2000);
            entity.Property(e => e.LinkUrl).HasMaxLength(500);
            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
