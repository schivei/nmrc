using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nmrc.Control;
using Nmrc.Control.Constants;
using System;

namespace Nmrc.Controllers
{
    [ApiController]
    [Route("/rest/mars")]
    public class ControlController : ControllerBase
    {
        private readonly ILogger<ControlController> _logger;

        public ControlController(ILogger<ControlController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{command}")]
        public IActionResult RunCommand([FromRoute] string command, [FromServices] GridArea grid)
        {
            try
            {
                var position = grid.Execute(command);
                return Ok($"({position.x}, {position.y}, {OrientationString(position.o)})");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("command", ex.Message);
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        private char OrientationString(Orientation o) =>
            o.ToString()[0];
    }
}
