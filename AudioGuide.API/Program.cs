using AudioGuide.BLL.Services;
using AudioGuide.DAL;
using AudioGuide.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuide.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioGuidesController : ControllerBase
{
    private readonly IAudioGuideService _service;
    private readonly AppDbContext _context;

    public AudioGuidesController(IAudioGuideService service, AppDbContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? lang)
    {
        var data = await _service.GetAllAsync(lang);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AudioGuideItem item)
    {
        try
        {
            // Đảm bảo Id được tự sinh (không bị gán cố định)
            item.Id = 0;

            _context.AudioGuides.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.AudioGuides.FindAsync(id);
        if (item == null) return NotFound();

        _context.AudioGuides.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Đã xóa thành công bài thuyết minh id {id}" });
    }
}