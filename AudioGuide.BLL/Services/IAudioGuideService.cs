using AudioGuide.BLL.DTOs;

namespace AudioGuide.BLL.Services;

public interface IAudioGuideService
{
    Task<IEnumerable<AudioGuideDto>> GetAllAsync(string? lang);
    Task<IEnumerable<AudioGuideDto>> GetByLanguageAsync(string lang);
    Task<AudioGuideDto?> GetByIdAsync(int id);
    Task<AudioGuideDto> CreateAsync(AudioGuideDto dto);
    Task<bool> DeleteAsync(int id);
}