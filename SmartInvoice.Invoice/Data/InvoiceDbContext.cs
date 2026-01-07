using Microsoft.EntityFrameworkCore;
using SmartInvoice.Invoice.Models;
using System.Reflection.Emit;

namespace SmartInvoice.Invoice.Data
{
    public class InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : DbContext(options)
    {
        // Use fully qualified names to resolve ambiguity
        public DbSet<Models.Invoice> Invoices { get; set; }
        public DbSet<Models.InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Models.Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Invoice configuration
            modelBuilder.Entity<Models.Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.DueDate);

                entity.Property(e => e.InvoiceNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(200);

                entity.Property(e => e.CustomerEmail)
                    .HasMaxLength(200);

                entity.Property(e => e.SubTotal)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TaxAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TotalAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.Status)
                    .HasConversion<string>();

                // Relationships
                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Payments)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // InvoiceItem configuration
            modelBuilder.Entity<Models.InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Quantity)
                    .HasPrecision(18, 2);

                entity.Property(e => e.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(e => e.SubTotal)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountAmount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.Total)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountPercentage)
                    .HasPrecision(5, 2);

                entity.Property(e => e.UnitType)
                    .HasMaxLength(50)
                    .HasDefaultValue("unit");
            });

            // Payment configuration
            modelBuilder.Entity<Models.Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TransactionId).IsUnique();

                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100);

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.Notes)
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .HasConversion<string>();
            });
        }
    }
}