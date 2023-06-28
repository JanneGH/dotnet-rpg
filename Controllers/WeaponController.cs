using dotnet_rpg.DTOs.Character;
using dotnet_rpg.DTOs.Weapon;
using dotnet_rpg.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetCharacterResponseDto>>> AddWeapon(AddWeaponRequestDto newWeapon)
        {
            return Ok(await _weaponService.AddWeapon(newWeapon));
        }
    }
}

/// Learned:
/// The Microsoft EF tutorial uses return NoContent() instead of Ok(something).
/// NoContent: returns a statuscodeResult 204, which sets the status code on the response.
/// Ok(something) rturns an OkObjectResult(something).
///    When executed, it reaches the OutputFormatterSelector which iterates over outputformatters in order and returns the first one that has a 'true' value or null (if the OutputFormatter is set to TreatNullValueAsNoContent (default true) AND the Object to return is null too). So it generates more code. 
///    Microsoft documentation: Ok(something) performs content negotiation when executed, formats the entity body and will produce a status200OK response if all that succeeds.