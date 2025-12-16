using System.Collections.Concurrent;
using Api.Models;
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
            "LICENSE-2025-LOCAL",
            "TRIAL-KEY-0001"
        };

        private static readonly ConcurrentDictionary<string, DateTime> ActivatedKeys = new(StringComparer.OrdinalIgnoreCase);

        [HttpPost("activate")]
        public IActionResult Activate([FromBody] LicenseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.LicenseKey))
            {
                return BadRequest(new ActivationResponse
                {
                    Success = false,
                    Message = "License key is required."
                });
            }

            if (!ValidKeys.Contains(request.LicenseKey))
            {
                return BadRequest(new ActivationResponse
                {
                    Success = false,
                    Message = "Invalid license key."
                });
            }

            var activatedAt = ActivatedKeys.GetOrAdd(request.LicenseKey, _ => DateTime.UtcNow);

            return Ok(new ActivationResponse
            {
                Success = true,
                Message = "Activation successful",
                ActivatedAt = activatedAt,
                LicenseKey = request.LicenseKey
            });
        }
    }
}
