using Microsoft.AspNetCore.Mvc;
using WebCoffee.Models;

[Route("api/[controller]")]
[ApiController]
public class TopingController : ControllerBase
{
    private readonly IToppingRepository _topingRepository;

    public TopingController(IToppingRepository topingRepository)
    {
        _topingRepository = topingRepository;
    }

    [HttpGet("Getall")]
    public async Task<IActionResult> GetAllTopings()
    {
        var topings = await _topingRepository.GetAllToppingsAsync();
        return Ok(topings);
    }

    [HttpGet("Find{name}")]
    public async Task<IActionResult> GetToppingByNameAsync(string name)
    {
        var toping = await _topingRepository.GetToppingByNameAsync(name);
        if (toping == null)
            return NotFound();

        return Ok(toping);
    }

    [HttpPost("AddTopping")]
    public async Task<JsonResult> AddToping([FromForm] Toping toping)
    {
        var result = await _topingRepository.AddTopingAsync(toping);
        return result; // Trả về kết quả từ repository
    }

    [HttpPut("edit/{name}")]
    public async Task<IActionResult> EditToping(string name, [FromForm] Toping topingMD)
    {
        var result = await _topingRepository.EditTopingAsync(name, topingMD);

        if (result.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound(result.Value);
        }

        return Ok(result.Value);
    }

    [HttpDelete("Delete/{name}")]
    public async Task<IActionResult> DeleteToping(string name)
    {
        var result = await _topingRepository.DeleteToppingAsync(name);
        return Ok(result);
    }
}
