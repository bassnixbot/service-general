using GeneralServices.DB;
using GeneralServices.Models;
using Microsoft.AspNetCore.Mvc;
using UtilsLib;

namespace GeneralServices.Controllers;

[ApiController]
[Route("general")]
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
    public async Task<ActionResult> Fill(ClientInfo request)
    {
        var output = await Services.Services.BuildFill(request);
        if (!output.success)
            return StatusCode(500, output);

        return Ok(output);
    }

    [HttpPost("link")]
    public async Task<ActionResult> Link(ClientInfo request)
    {
        var output = await Services.Services.BuildLink(request, _context);
        if (!output.success)
            return StatusCode(500, output);
        
        return Ok(output);
    }

    [HttpPost("pyramid")]
    public async Task<ActionResult> Pyramid(ClientInfo request)
    {
        var output = await Services.Services.BuildPyramid(request);
        if (!output.success)
            return StatusCode(500, output);

        return Ok(output);
    }

    [HttpPost("tuck")]
    public async Task<ActionResult> Tuck(ClientInfo request)
    {
        var output = await Services.Services.BuildTuck(request);

        if (!output.success)
            return StatusCode(500, output);

        return Ok();
    }

    [HttpPost("urban")]
    public async Task<ActionResult> Urban(ClientInfo request)
    {
        var output = await Services.Services.BuildUrban(request);

        if (!output.success)
            return StatusCode(500, output);

        return Ok(output);
    }
}
