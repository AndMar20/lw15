using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LicenseController : ControllerBase
    {
        private static readonly HashSet<string> ValidKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "ABC123-XYZ789",
            "LICENSE-2025-LOCAL"
        };

        [HttpPost("activate")]
        public IActionResult Activate([FromBody] LicenseRequest request)
        {
            if (string.IsNullOrEmpty(request?.LicenseKey))
                return BadRequest("License key is required.");

            if (ValidKeys.Contains(request.LicenseKey))
                return Ok(new { message = "Activation successful" });
            else
                return BadRequest("Invalid license key."); // ← исправлено!
        }
    }
}
