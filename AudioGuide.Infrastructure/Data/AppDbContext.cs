using AudioGuide.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AudioGuide.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Poi> Pois => Set<Poi>();
        public DbSet<PoiTranslation> PoiTranslations => Set<PoiTranslation>();
        public DbSet<QrCode> QrCodes => Set<QrCode>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình bảng POI
            modelBuilder.Entity<Poi>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Code).HasMaxLength(50).IsRequired();
                entity.HasIndex(p => p.Code).IsUnique();
            });

            // 2. Cấu hình bảng POI Translations (Đa ngôn ngữ)
            modelBuilder.Entity<PoiTranslation>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.LanguageCode).HasMaxLength(10).IsRequired();
                entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
                entity.Property(t => t.AudioUrl).HasMaxLength(500).IsRequired();

                entity.HasIndex(t => new { t.PoiId, t.LanguageCode }).IsUnique();

                entity.HasOne(t => t.Poi)
                      .WithMany(p => p.Translations)
                      .HasForeignKey(t => t.PoiId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 3. Cấu hình bảng QR Code
            modelBuilder.Entity<QrCode>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.Property(q => q.QrToken).HasMaxLength(64).IsRequired();
                entity.HasIndex(q => q.QrToken).IsUnique();

                entity.HasOne(q => q.Poi)
                      .WithMany(p => p.QrCodes)
                      .HasForeignKey(q => q.PoiId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}