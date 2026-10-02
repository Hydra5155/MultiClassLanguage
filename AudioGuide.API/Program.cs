using AudioGuide.Core.Interfaces;
using AudioGuide.Core.Services;
using AudioGuide.Infrastructure.Data;
using AudioGuide.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình kết nối SQL Server và kích hoạt NetTopologySuite xử lý Point
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    ));

// 2. Đăng ký Dependency Injection
builder.Services.AddScoped<IPoiRepository, PoiRepository>();
builder.Services.AddScoped<IAudioGuideService, AudioGuideService>();

// 3. Đăng ký HttpClient Factory để stream âm thanh TTS trong QrGuideController
builder.Services.AddHttpClient();

// 4. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercelAndLocal", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Tự động tạo bảng & nạp dữ liệu seed nếu database trên SSMS chưa có
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowVercelAndLocal");

app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();