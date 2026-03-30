using InquirySpark.Repository.Services.UserPreferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InquirySpark.Admin.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserPreferencesController(IUserPreferenceService service, ILogger<UserPreferencesController> logger) : ControllerBase
{
    private readonly IUserPreferenceService _service = service;
    private readonly ILogger<UserPreferencesController> _logger = logger;

    [HttpGet("{userId}/{key}")]
    public async Task<IActionResult> Get(int userId, string key)
    {
        var value = await _service.GetPreferenceAsync(userId, key);
        
        if (value == null)
            return NotFound();
        
        return Ok(new { Key = key, Value = value });
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] UserPreferenceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        await _service.SavePreferenceAsync(request.UserId, request.Key, request.Value);
        
        return Ok();
    }

    [HttpDelete("{userId}/{key}")]
    public async Task<IActionResult> Delete(int userId, string key)
    {
        await _service.DeletePreferenceAsync(userId, key);
        return NoContent();
    }
}

public class UserPreferenceRequest
{
    public int UserId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
