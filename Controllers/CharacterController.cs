using System.Security.Claims;
using dotnet_rpg.DTOs.Character;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [ApiController]
    // TODO: Check openApi standards
    [Route("[controller]")]
    /// ControllerBase class has no view support.
    /// View support not needed as we build an API.
    /// For View support use Controller base class.
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        /// SUMMARY about GET methods in an API ///
        ///Even though WebApi supports naming conventions and will assume a GET request if you start it with "Get", 
        /// And even though it will work if you only have 1 GET method,
        /// the OpenApi standards ask for an HttpGet attribute.
        /// Just the attribute wil work if you only have 1 GET method.
        /// You need routing attributes when you have more so the API knows which one to use.

        // TODO: Check openApi standards
        ///[AllowAnonymous] /// fun test: to make an exception to the general Authorize attribute
        [HttpGet("GetAll")]
        /// If you use just IActionResult Get(), Swagger shows no schemas or expected results. 
        /// That is why ActionResult<T> is used.
        public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> Get()
        {
            // TODO: Add code if get fails
            // TODO: Add logging
            return Ok(await _characterService.GetAllCharacters());
        }

        // Send the data via the URL (not the body of the request)
        // TODO: Check openApi standards
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> GetSingleCharacter(int id)
        {
            // TODO: Add code if get fails
            // TODO: Add logging
            // Using LINQ to get Character with Id 1
            return Ok(await _characterService.GetCharacterById(id));
        }

        // Create new character.
        /// The client (browser) sends JSON objects to service, 
        /// service creates new character based on the JSON data.
        /// The data (JSON object) is sent through the body of the request.
        [HttpPost("AddCharacter")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut("UpdateCharacter")]
        public async Task<ActionResult<ServiceResponse<GetCharacterResponseDto>>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
        {
            var response = await _characterService.UpdateCharacter(updatedCharacter);

            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterResponseDto>>>> DeleteCharacter(int id)
        {
            // TODO: Add code if get fails
            // TODO: Add logging
            var response = await _characterService.DeleteCharacter(id);

            if (response.Data is null)
            {
                return NotFound(response);
            }

            return Ok();
        }
    }
}

/// SUMMARY of GET, POST, PUT, DELETE ///
/// GET:
/// Requests a representation of the specified resource.
/// POST: 
/// Submit an entity to the specified resource, 
/// often causing a change in state or side effects on the server.
/// PUT:
/// Replaces ALL current respresentations of the target resource with the request payload.
/// There are ways to influence this, but normally you send the entire object even if you only want to change one property.
/// DELETE:
/// Deletes the specified resource. Soft delete would be a PUT method.

/// Back when the userId was passed as a parameter from the controller to the service,
/// a specific "User" was called: 
/// Note that User here is the ControllerBase (native) User object!
/// Gets the System.Security.Claims.ClaimsPrincipal for user associated with the
/// executing action.
/// Only recieve characters that belong to a specific user.
/// int userId = int.Parse(User.Claims.FirstOrDefault(claims =>
/// claims.Type == ClaimTypes.NameIdentifier)!.Value);