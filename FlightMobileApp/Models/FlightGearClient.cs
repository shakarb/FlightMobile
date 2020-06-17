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
                    _tcp.write(command.Command);
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
