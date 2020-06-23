using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public enum Result { Ok, NotOk }
    /// <summary>
    ///  Command class to hold the incoming command json values
    /// </summary>
    public class Command {

        public double aileron { get; set; } 

        public double rudder { get; set; }

        public double throttle { get; set; }

        public double elevator { get; set; }
    }


    /// <summary>
    /// Async Command class for post requst from client
    /// </summary>
    public class AsyncCommand
    {
        public Command Command { get; private set; }

        public TaskCompletionSource<Result> Completion { get; private set; }

        public Task<Result> Task { get => Completion.Task; }

        public AsyncCommand(Command input)
        {
            Command = input;

            Completion = new TaskCompletionSource<Result>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
