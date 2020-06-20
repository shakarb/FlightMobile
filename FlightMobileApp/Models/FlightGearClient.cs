using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class FlightGearClient : IFlightGearClient
    {
        private readonly BlockingCollection<AsyncCommand> _queue;

        private readonly ITelnetClient _tcp;

        //Constructor
        public FlightGearClient(ITelnetClient tcp)
        {
            _tcp = tcp;
            _queue = new BlockingCollection<AsyncCommand>();
        }
        //This function is called by the WEbApi controller , it will await on the retunr Task<>
        //This isnt an async method , not await.
        public Task<Result> Execute(Command cmd)
        {
            var asyncCommand = new AsyncCommand(cmd);
            _queue.Add(asyncCommand);
            return asyncCommand.Task;
        }

        public void Start()
        {
            Task.Factory.StartNew(ProcessCommands);
        }

        private void ProcessCommands()
        {
            //NetworkStream stream = _client.GetStream();

            foreach(AsyncCommand command in _queue.GetConsumingEnumerable())
            {
                Result res;
                try 
                {
        
                    _tcp.write("set /controls/engines/current-engine/throttle " + String.Format("{0:0.##}", command.Command.throttle) + "\r\n");
                    _tcp.write("set /controls/flight/rudder " + String.Format("{0:0.##}", command.Command.rudder) + "\r\n");
                    _tcp.write("set /controls/flight/aileron " + String.Format("{0:0.##}", command.Command.aileron) + "\r\n");
                    _tcp.write("set /controls/flight/elevator " + String.Format("{0:0.##}", command.Command.elevator) + "\r\n");

                    double aileron = Double.Parse(_tcp.read("get /controls/flight/aileron"));
                    double rudder = Double.Parse(_tcp.read("get /controls/flight/rudder"));
                    double elevator = Double.Parse(_tcp.read("get /controls/flight/elevator"));
                    double throttle = Double.Parse(_tcp.read("get /controls/engines/current-engine/throttle"));
                    //check validation
                    if (aileron != command.Command.aileron || rudder != command.Command.rudder ||
                            elevator != command.Command.elevator || throttle != command.Command.throttle)
                    {
                        res = Result.NotOk;
                    }
                    res = Result.Ok;
                }
                catch
                {
                    res = Result.NotOk; 
                }
                //Check is value defined and then set Result  
                command.Completion.SetResult(res);
            }
        }
    }
}
