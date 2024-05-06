using GeneralServices.DB;
using GeneralServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeneralServices.Controllers;

[ApiController]
[Route("[controller]")]
public class GeneralController : ControllerBase
{
    private readonly ILogger<GeneralController> _logger;
    private readonly ApplicationDBContext _context;
    private int messagelimit = 500;

    public GeneralController(ILogger<GeneralController> logger, ApplicationDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("fill")]
    public async Task<ActionResult> Fill(string message)
    {
        var output = await Services.Services.BuildFill(message);
        if (!output.success)
            return StatusCode(500, output);

        return Ok(output);
    }

    [HttpPost("link")]
    public async Task<ActionResult> Link(string message, ChatterInformation chatterInfo)
    {
        var output = await Services.Services.BuildLink(message, chatterInfo, _context);
        if (!output.success)
            return StatusCode(500, output);
        
        return Ok(output);
    }

    [HttpPost("pyramid")]
    public async Task<ActionResult> Pyramid(string message)
    {
        var output = await Services.Services.BuildPyramid(message);
        if (!output.success)
            return StatusCode(500, output);

        return Ok(output);
    }

    [HttpPost("tuck")]
    public async Task<ActionResult> Tuck(string message)
    {
        var output = await Services.Services.BuildTuck(message);

        if (!output.success)
            return StatusCode(500, output);

        return Ok();
    }

    [HttpPost("urban")]
    public async Task<ActionResult> Urban(string message)
    {
        var output = await Services.Services.BuildUrban(message);

        if (!output.success)
            return StatusCode(500, output);

        return Ok(output);
    }
}
