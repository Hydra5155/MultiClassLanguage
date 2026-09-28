using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AudioGuide.Tests;

[TestClass]
public class AudioGuideLogicTests
{
    // Giả lập hàm kiểm tra mã ngôn ngữ hợp lệ từ hệ thống thuyết minh
    private bool IsSupportedLanguage(string langCode)
    {
        if (string.IsNullOrWhiteSpace(langCode)) return false;
        var supported = new[] { "vi", "en", "ja", "fr", "ko", "zh" };
        return supported.Contains(langCode.ToLower());
    }

    [TestMethod]
    public void IsSupportedLanguage_ValidCode_ReturnsTrue()
    {
        // Arrangae
        string lang = "vi";

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
}