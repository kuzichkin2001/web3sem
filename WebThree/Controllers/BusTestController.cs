using Bus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebThree.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusTestController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusTestController(IBusService busService)
        {
            _busService = busService;
        }

        [HttpPost]
        [Route("to/web3.1")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageToWeb31(
            [FromQuery] string message,
            CancellationToken ct)
        {
            try
            {
                await _busService.SendMessageAsync(message, "Web3.1", ct);

                return Ok();
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("to/web3.2")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageToWeb32(
            [FromQuery] string message,
            CancellationToken ct)
        {
            try
            {
                await _busService.SendMessageAsync(message, "Web3.2", ct);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("to/web3.3")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageToWeb33(
            [FromQuery] string message,
            CancellationToken ct)
        {
            try
            {
                await _busService.SendMessageAsync(message, "Web3.3", ct);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
