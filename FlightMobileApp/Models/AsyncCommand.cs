using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public enum Result { Ok, NotOk }

    public class Command {

        public double aileron { get; set; } 

        public double rudder { get; set; }

        public double throttle { get; set; }

        public double elevator { get; set; }

        public string CommandToSet()
        {
            string command =
                "set /controls/engines/current-engine/throttle " + this.throttle.ToString() + "\r\n"
                + "set /controls/flight/rudder " + String.Format("{0:0.##}", this.rudder) + "\r\n"
                + "set /controls/flight/aileron " + this.aileron.ToString() + "\r\n"
                + "set /controls/flight/elevator " + this.elevator.ToString() + "\r\n";
            return command;
        }
    }

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
