using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public interface IFlightGearClient
    {
        public Task<Result> Execute(Command cmd);
        public void Start();
    }
}
