using AudioGuide.BLL.DTOs;
using AudioGuide.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuide.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioGuidesController : ControllerBase
{
    private readonly IAudioGuideService _service;

    public AudioGuidesController(IAudioGuideService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? lang)
    {
        var data = await _service.GetAllAsync(lang);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "ID không hợp lệ." });
        }

        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound(new { message = $"Không tìm thấy bài thuyết minh có ID = {id}." });
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AudioGuideDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest(new { message = "Tiêu đề không được để trống." });
        }

        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, detail = ex.InnerException?.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "ID không hợp lệ." });
        }

        var result = await _service.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Không tìm thấy bài thuyết minh có ID = {id} để xóa." });
        }

        return Ok(new { message = $"Đã xóa thành công bài thuyết minh có ID = {id}." });
    }
}