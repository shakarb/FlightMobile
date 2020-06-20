using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public interface ITelnetClient
    {
        void connect();
        void write(string command);
        string read(string command); //blocking call 
        void disconnect();
        bool isConnected();
    }
}
