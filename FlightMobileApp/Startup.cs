using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlightMobileApp
{
    public class Startup
    {
        private IConfiguration _config;


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _config = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //get the ip and port configuration
            string ip = _config["Ip"]; // get the ip from appsettings.json
             int port = Int32.Parse(_config["Port"]); // get the port from appsettings.json

            //Add the TelnetClient
            ITelnetClient tlc = new MyTelnetClient(ip , port);
            services.AddSingleton<ITelnetClient>(tlc);

            //Add singleton FlightGearClient 
            IFlightGearClient fgc = new FlightGearClient(tlc);
            services.AddSingleton<IFlightGearClient>(fgc);
            //Strting going over the blocking queue
            fgc.Start();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();
            

            app.UseDefaultFiles();

            ////redirect GET /screenshot to the simulator http server
            //app.Use(async (context, next) =>
            //{
            //    var url = context.Request.Path.Value;

            //    // Redirect to an external URL
            //    if (url.Contains("/screenshot"))
            //    {
            //        context.Response.Redirect(_config["HTTPSimulatorIP"] +":" + _config["HTTPSimulatorPort"] + "/screenshot");
            //        return; 
            //    }

            //    await next();
            //});


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
