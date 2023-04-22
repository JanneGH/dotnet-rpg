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
        private static Character knight = new Character();

        // Even though WebApi supports naming conventions and will assume a GET request if you start it with "Get", 
        // And even though we have only 1 GET method so far which technically works,
        // the OpenApi standards ask for an HttpGet attribute.
        // Just the attribute wil work but I named it as well.
        [HttpGet(Name = "GetCharacter")]
        // If you use just IActionResult Get(), Swagger shows no schemas or expected results. 
        // That is why ActionResult<T> is used.
        public ActionResult<Character> Get()
        {
            //TODO: Add code if get fails
            return Ok(knight);
        }
    }
}