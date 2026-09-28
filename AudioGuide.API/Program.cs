using AudioGuide.BLL.Services;
using AudioGuide.DAL;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace AudioGuide.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // 1. Cấu hình DbContext an toàn
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
        else
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("AudioGuideDb"));
        }

        builder.Services.AddScoped<IAudioGuideService, AudioGuideService>();

        // 2. Cấu hình CORS mở hoàn toàn
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // 3. Khởi tạo dữ liệu mẫu
        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization note: {ex.Message}");
            }
        }

        // 4. Kích hoạt Scalar UI
        app.MapOpenApi();
        app.MapScalarApiReference();

        // Bật CORS cho toàn bộ ứng dụng
        app.UseCors();

        // LƯU Ý: KHÔNG gọi app.UseHttpsRedirection() khi chạy trong container Docker trên Render

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}