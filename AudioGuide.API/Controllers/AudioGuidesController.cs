using AudioGuide.BLL.DTOs;
using AudioGuide.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuide.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioGuidesController : ControllerBase
{
    private readonly IAudioGuideService _audioService;

    public AudioGuidesController(IAudioGuideService audioService)
    {
        _audioService = audioService;
    }

    // GET /api/audioguides?lang=vi
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string lang = "vi")
    {
        var result = await _audioService.GetByLanguageAsync(lang);
        return Ok(result);
    }

    // POST /api/audioguides
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AudioGuideDto dto)
    {
        var created = await _audioService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { lang = created.LanguageCode }, created);
    }
}
[HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAudioGuide(int id)
    {
        var item = await _context.AudioGuides.FindAsync(id);
        if (item == null)
        {
            return NotFound(new { message = "Không tìm thấy bản ghi để xóa." });
        }

        _context.AudioGuides.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent(); // Trả về mã 204 No Content xóa thành công
    }