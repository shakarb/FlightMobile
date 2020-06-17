using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightMobileApp.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private IFlightGearClient myModel;
        public CommandController(IFlightGearClient fgc)
        {
            myModel = fgc;
        }

        // GET: api/Command
        [HttpGet]
        public ActionResult<int> Get()
        {
            // Returns 200 status code.
            return Ok(1);
        }

        // POST: api/Command
        [HttpPost]
        async public Task<int> Post([FromBody] Command command)
        {
            try
            {
                Result res = await myModel.Execute(command);
                if(res == Result.Ok)
                {
                    return StatusCodes.Status200OK;
                } else
                {
                    return StatusCodes.Status400BadRequest;
                }
            }
            catch
            {
                return StatusCodes.Status400BadRequest;
            }
        }
    }
}
