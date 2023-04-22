using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    // TODO: Check openApi standards
    [Route("api/[controller]")]
    // ControllerBase class has no view support.
    // View support not needed as we build an API.
    // For View support use Controller base class.
    public class CharacterController : ControllerBase
    {
        private static List<Character> characters = new List<Character>() {
            new Character(),
            new Character { Id = 1, Name = "Ali Baba" }
        };

        /// SUMMARY about GET methods in an API ///
        //Even though WebApi supports naming conventions and will assume a GET request if you start it with "Get", 
        // And even though it will work if you only have 1 GET method,
        // the OpenApi standards ask for an HttpGet attribute.
        // Just the attribute wil work if you only have 1 GET method.
        // You need routing attributes when you have more so the API knows which one to use.

        // TODO: Check openApi standards
        [HttpGet("GetAllCharacters")]
        // If you use just IActionResult Get(), Swagger shows no schemas or expected results. 
        // That is why ActionResult<T> is used.
        public ActionResult<List<Character>> GetAllCharacters()
        {
            // TODO: Add code if get fails
            // TODO: Add logging
            return Ok(characters);
        }

        // We send the data via the URL (not the body of the request)
        // TODO: Check openApi standards
        [HttpGet("{id}")]
        public ActionResult<List<Character>> GetSingleCharacter(int id)
        {
            // TODO: Add code if get fails
            // TODO: Add logging
            // Using LINQ to get Character with Id 1
            return Ok(characters.FirstOrDefault(character =>
                character.Id == id
            ));
        }

        // Create new character.
        // The client (browser) sends JSON objects to service, 
        // service creates new character based on the JSON data.
        // The data (JSON object) is sent through the body of the request.
        [HttpPost("AddCharacter")]
        public ActionResult<List<Character>> AddCharacter(Character newCharacter)
        {
            characters.Add(newCharacter);
            return Ok(characters);
        }
    }
}

/// SUMMARY of GET, POST, PUT, DELETE ///
// GET:
// Requests a representation of the specified resource.
// POST: 
// Submit an entity to the specified resource, 
// often causing a change in state or side effects on the server.
// PUT:
// Replaces ALL current respresentations of the target resource with the request payload.
// There are ways to influence this, but normally you send the entire object even if you only want to change one property.
// DELETE:
// Deletes the specified resource. Soft delete would be a PUT method.