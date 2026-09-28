using Microsoft.EntityFrameworkCore;
using AudioGuide.DAL.Entities;

namespace AudioGuide.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Khởi tạo Set để tránh cảnh báo CS8618
    public DbSet<AudioGuideItem> AudioGuides => Set<AudioGuideItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình bảng AudioGuides
        modelBuilder.Entity<AudioGuideItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(250);
            entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(10);
            entity.Property(e => e.AudioUrl).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Transcript).IsRequired();
        });

        // Nạp sẵn dữ liệu thuyết minh thực tế
        modelBuilder.Entity<AudioGuideItem>().HasData(
            new AudioGuideItem
            {
                Id = 1,
                Title = "Dinh Độc Lập - Hội trường Thống Nhất",
                LanguageCode = "vi",
                AudioUrl = "https://actions.google.com/sounds/v1/ambiences/outdoor_market.ogg",
                Transcript = "Dinh Độc Lập là một di tích lịch sử nổi tiếng tại TP. Hồ Chí Minh, lưu giữ nhiều giá trị văn hóa và kiến trúc độc đáo."
            },
            new AudioGuideItem
            {
                Id = 2,
                Title = "Independence Palace",
                LanguageCode = "en",
                AudioUrl = "https://actions.google.com/sounds/v1/ambiences/outdoor_market.ogg",
                Transcript = "The Independence Palace is an iconic historical landmark in Ho Chi Minh City, preserving architectural and cultural heritage."
            },
            new AudioGuideItem
            {
                Id = 3,
                Title = "Chùa Một Cột",
                LanguageCode = "vi",
                AudioUrl = "https://actions.google.com/sounds/v1/water/creek_water.ogg",
                Transcript = "Chùa Một Cột được xây dựng từ thời vua Lý Thái Tông năm 1049, sở hữu kiến trúc tựa đóa hoa sen thanh tịnh."
            },
            new AudioGuideItem
            {
                Id = 4,
                Title = "One Pillar Pagoda",
                LanguageCode = "en",
                AudioUrl = "https://actions.google.com/sounds/v1/water/creek_water.ogg",
                Transcript = "The One Pillar Pagoda was built in 1049 under Emperor Ly Thai Tong, designed to resemble a pure lotus blossom."
            }
        );
    }
}