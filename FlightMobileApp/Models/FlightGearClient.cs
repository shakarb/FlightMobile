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
        /// <summary>
        ///  Contructor, getting argument threw DI.
        /// </summary>
        public FlightGearClient(ITelnetClient tcp)
        {
            _tcp = tcp;
            _queue = new BlockingCollection<AsyncCommand>();
        }
        /// <summary>
        ///This function is called by the WEbApi controller , it will await on the retunr Task<>
        ///This isnt an async method , not await.
        /// </summary>

        public Task<Result> Execute(Command cmd)
        {
            var asyncCommand = new AsyncCommand(cmd);
            _queue.Add(asyncCommand);
            return asyncCommand.Task;
        }
        /// <summary>
        ///Startin the queue decueing 
        /// </summary>
        public void Start()
        {
            Task.Factory.StartNew(ProcessCommands);
        }

        /// <summary>
        ///  Processing incoming command
        /// </summary>
        private void ProcessCommands()
        {
            //NetworkStream stream = _client.GetStream();

            foreach(AsyncCommand command in _queue.GetConsumingEnumerable())
            {
                Result res;
                try 
                {
                    //Sending data to the simulator
                    _tcp.write("set /controls/engines/current-engine/throttle " + String.Format("{0:0.##}", command.Command.throttle) + "\r\n");
                    _tcp.write("set /controls/flight/rudder " + String.Format("{0:0.##}", command.Command.rudder) + "\r\n");
                    _tcp.write("set /controls/flight/aileron " + String.Format("{0:0.##}", command.Command.aileron) + "\r\n");
                    _tcp.write("set /controls/flight/elevator " + String.Format("{0:0.##}", command.Command.elevator) + "\r\n");
                    
                    //check validation
                    res = ValidateData(command.Command);
                }
                catch
                {
                    res = Result.NotOk; 
                }
                //Check is value defined and then set Result  
                command.Completion.SetResult(res);
            }
        }

        /// <summary>
        ///  Validate the data we send to the Simulator
        /// </summary>
        private Result ValidateData(Command command)
        {
            //Get data from simulator
            double aileron = Double.Parse(_tcp.read("get /controls/flight/aileron"));
            double rudder = Double.Parse(_tcp.read("get /controls/flight/rudder"));
            double elevator = Double.Parse(_tcp.read("get /controls/flight/elevator"));
            double throttle = Double.Parse(_tcp.read("get /controls/engines/current-engine/throttle"));

            if (aileron != Convert.ToDouble(String.Format("{0:0.##}", command.aileron))
                        || rudder != Convert.ToDouble(String.Format("{0:0.##}", command.rudder))
                        || elevator != Convert.ToDouble(String.Format("{0:0.##}", command.elevator))
                        || throttle != Convert.ToDouble(String.Format("{0:0.##}", command.throttle)))
            {
                return Result.NotOk;
            } else
            {
                return Result.Ok;
            }
        }
    }
}
