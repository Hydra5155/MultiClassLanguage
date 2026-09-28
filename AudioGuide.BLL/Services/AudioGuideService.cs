using AudioGuide.BLL.DTOs;
using AudioGuide.DAL;
using AudioGuide.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AudioGuide.BLL.Services;

public class AudioGuideService : IAudioGuideService
{
    private readonly AppDbContext _context;

    public AudioGuideService(AppDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả hoặc lọc theo ngôn ngữ nếu có truyền lang
    public async Task<IEnumerable<AudioGuideDto>> GetAllAsync(string? lang)
    {
        var query = _context.AudioGuides.AsQueryable();

        if (!string.IsNullOrWhiteSpace(lang))
        {
            var targetLang = lang.Trim().ToLower();
            query = query.Where(x => x.LanguageCode.ToLower() == targetLang);
        }

        return await query
            .Select(x => new AudioGuideDto(x.Id, x.Title, x.LanguageCode, x.AudioUrl, x.Transcript))
            .ToListAsync();
    }

    // Lọc bắt buộc theo ngôn ngữ
    public async Task<IEnumerable<AudioGuideDto>> GetByLanguageAsync(string lang)
    {
        var targetLang = string.IsNullOrWhiteSpace(lang) ? "vi" : lang.Trim().ToLower();

        return await _context.AudioGuides
            .Where(x => x.LanguageCode.ToLower() == targetLang)
            .Select(x => new AudioGuideDto(x.Id, x.Title, x.LanguageCode, x.AudioUrl, x.Transcript))
            .ToListAsync();
    }

    // Tìm chi tiết một bản ghi theo Id
    public async Task<AudioGuideDto?> GetByIdAsync(int id)
    {
        var entity = await _context.AudioGuides.FindAsync(id);
        if (entity == null) return null;

        return new AudioGuideDto(entity.Id, entity.Title, entity.LanguageCode, entity.AudioUrl, entity.Transcript);
    }

    // Tạo mới bản ghi từ DTO
    public async Task<AudioGuideDto> CreateAsync(AudioGuideDto dto)
    {
        var entity = new AudioGuideItem
        {
            Title = dto.Title,
            LanguageCode = string.IsNullOrWhiteSpace(dto.LanguageCode) ? "vi" : dto.LanguageCode.Trim().ToLower(),
            AudioUrl = dto.AudioUrl,
            Transcript = dto.Transcript
        };

        _context.AudioGuides.Add(entity);
        await _context.SaveChangesAsync();

        return new AudioGuideDto(entity.Id, entity.Title, entity.LanguageCode, entity.AudioUrl, entity.Transcript);
    }

    // Xóa bản ghi theo Id
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.AudioGuides.FindAsync(id);
        if (entity == null) return false;

        _context.AudioGuides.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}