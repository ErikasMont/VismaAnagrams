using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WordController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> DownloadDictionary()
    {
        var fileName = "zodynas.txt";
        if (!System.IO.File.Exists(fileName))
        {
            return NotFound("File does not exist.");
        }
        var bytes = await System.IO.File.ReadAllBytesAsync(fileName);

        return File(bytes, "text/plain", Path.GetFileName(fileName));
    }
}