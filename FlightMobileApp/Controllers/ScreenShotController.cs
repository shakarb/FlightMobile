using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FlightMobileApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScreenShotController : ControllerBase
    {
        IConfiguration _configuration;
        public ScreenShotController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/ScreenShot
        [HttpGet]
        public async Task<System.IO.Stream> Get()
        {
            HttpClient client = new HttpClient();
            string getCommand = "http://" + _configuration["HTTPSimulatorIP"] + ":"
                + _configuration["HTTPSimulatorPort"] + "/screenshot";
            try
            {
                //making a get request from simulator http server
                HttpResponseMessage respMsg = await client.GetAsync(getCommand);
                respMsg.EnsureSuccessStatusCode();
                System.IO.Stream responseBody = await respMsg.Content.ReadAsStreamAsync();
                if (respMsg.IsSuccessStatusCode)
                {
                    return responseBody;
                }
                else
                {
                    // requst didnt successed
                    Console.WriteLine("Error with get screenshot");
                    Response.StatusCode = 400;
                    client.Dispose();
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                // we face and error
                Console.WriteLine("Error with get screenshot:{0}", e.Message);
                Response.StatusCode = 400;
                client.Dispose();
                return null;
            }
        }
    }
}
