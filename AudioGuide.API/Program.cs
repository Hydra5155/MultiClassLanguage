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

        // 1. Thêm Controllers và OpenAPI
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // 2. Cấu hình DbContext linh hoạt:
        // Nếu có chuỗi kết nối DefaultConnection thì kết nối SQL Server, ngược lại dùng In-Memory Database
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

        // 3. Đăng ký Dependency Injection cho tầng BLL
        builder.Services.AddScoped<IAudioGuideService, AudioGuideService>();

        // 4. Cấu hình CORS để Vercel Web App gọi API không bị chặn
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // 5. Khởi tạo dữ liệu ban đầu an toàn (không làm crash ứng dụng trên Cloud)
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

        // 6. Cho phép mở tài liệu Scalar cả ở môi trường Development và Production
        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseHttpsRedirection();

        // Kích hoạt CORS
        app.UseCors("AllowAll");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}