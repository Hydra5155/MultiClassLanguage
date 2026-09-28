using AudioGuide.BLL.DTOs;
using AudioGuide.BLL.Services;
using AudioGuide.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AudioGuide.Tests;

[TestClass]
public class AudioGuideLogicTests
{
    // Giả lập hàm kiểm tra mã ngôn ngữ hợp lệ từ hệ thống thuyết minh
    private bool IsSupportedLanguage(string langCode)
    {
        if (string.IsNullOrWhiteSpace(langCode)) return false;
        var supported = new[] { "vi", "en", "ja", "fr", "ko", "zh" };
        return supported.Contains(langCode.Trim().ToLower());
    }

    [TestMethod]
    public void IsSupportedLanguage_ValidCode_ReturnsTrue()
    {
        // Arrange
        string lang = "vi";

        // Act
        bool result = IsSupportedLanguage(lang);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsSupportedLanguage_UppercaseValidCode_ReturnsTrue()
    {
        // Arrange
        string lang = "EN";

        // Act
        bool result = IsSupportedLanguage(lang);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsSupportedLanguage_InvalidCode_ReturnsFalse()
    {
        // Arrange
        string lang = "xyz";

        // Act
        bool result = IsSupportedLanguage(lang);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsSupportedLanguage_EmptyOrWhitespace_ReturnsFalse()
    {
        // Assert
        Assert.IsFalse(IsSupportedLanguage(""));
        Assert.IsFalse(IsSupportedLanguage("   "));
        Assert.IsFalse(IsSupportedLanguage(null!));
    }

    [TestMethod]
    public async Task Service_CreateAndGetByLanguage_ReturnsCorrectData()
    {
        // Arrange: Khởi tạo DbContext InMemory cho test
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var service = new AudioGuideService(context);

        var newGuide = new AudioGuideDto(0, "Test Pagoda", "en", "https://audio.link/1.mp3", "Transcript test");

        // Act
        var created = await service.CreateAsync(newGuide);
        var results = await service.GetByLanguageAsync("en");

        // Assert
        Assert.IsNotNull(created);
        Assert.AreEqual("Test Pagoda", created.Title);
        Assert.IsTrue(results.Any(x => x.Title == "Test Pagoda"));
    }
}