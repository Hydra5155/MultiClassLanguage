using AudioGuide.BLL.Services;
using AudioGuide.DAL;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace AudioGuide.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        // Cấu hình OpenAPI: Xóa Servers URL để Scalar tự động trỏ đúng domain HTTPS trên Render
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Servers.Clear();
                return Task.CompletedTask;
            });
        });

        // 1. Cấu hình DbContext dùng InMemory Database để test nhanh
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // 2. Đăng ký Dependency Injection cho tầng BLL
        builder.Services.AddScoped<IAudioGuideService, AudioGuideService>();

        // 3. Cấu hình CORS mở cho Web (React) và Mobile (React Native)
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

        // Nhận diện giao thức HTTPS đằng sau reverse proxy của Render
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        // Tự động nạp dữ liệu mẫu vào InMemory Database khi khởi động
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        // Bật OpenAPI và giao diện Scalar
        app.MapOpenApi();
        app.MapScalarApiReference();

        // Kích hoạt CORS
        app.UseCors("AllowAll");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}