using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlightMobileApp.Models
{
    public class MyTelnetClient : ITelnetClient
    {
        enum ConnectionStatus
        {
            NoConnection = 0,
            ConnectionFailed,
            Connected
        }

        private TcpClient client;
        private string ip;
        private int port;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        bool connected;

        private static Mutex mutex = new Mutex();
        public int connectionStatus { get; set; }



        public MyTelnetClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            connect();
        }


        public void connect()
        {
            // Message.
            Console.WriteLine("Establishing connection to ip: {0} by port: {1}\n", ip, port);

            // Create client.
            client = new TcpClient();

            //set time out
            client.ReceiveTimeout = 10000; //10 sec time out.
            
            // Try to connect.
            try
            {
                client.Connect(ip, port);
                Console.WriteLine("\n\n***Connection established :) ***\n\n");
                connectionStatus = (int)ConnectionStatus.Connected;
                connected = true;
                //defining writer and reader for stream
                NetworkStream stream = client.GetStream();
                streamWriter = new StreamWriter(stream);
                streamWriter.AutoFlush = true;
                streamReader = new StreamReader(stream);
                //first command when we connect
                streamWriter.WriteLine("data\n");
            }
            catch (Exception ex)
            {
                // If connection failed
                connectionStatus = (int)ConnectionStatus.ConnectionFailed;
                Console.WriteLine("Error occur on simulator connection" + ex.StackTrace);
            }
        }

        public void disconnect()
        {
            client.Client.Close();
            Console.WriteLine("\nDisconnecting server\n");
            connected = false;
        }

        public bool isConnected()
        {
            return connected;
        }

        /// <summary>
        /// Reads data from server (according to given "get" command).
        /// </summary>
        /// <param name="command">  The "get" command to the server </param>
        /// <returns></returns>
        public string read(string command)
        {
            //initializing data string
            string data;
            try
            {
                // write the data command to the simulator
                streamWriter.WriteLine(command);
                // Reading.
                //byte[] read = new byte[1024];
                data = streamReader.ReadLine();
                // Here we read the server response so the server (the buffer) won't be with unnecessary information
                // that another thread by mistake can read instead of necessary information.
                //client.GetStream().Read(read, 0, 1024);
                //data = Encoding.ASCII.GetString(read, 0, read.Length);

                return data;
            }
            catch (IOException io)
            {
                // Moving the treatment in this exception upwards.
                throw new IOException();
            }
            catch (Exception exception)
            {
                Console.WriteLine("\n\n\n\n\n****** Reading got exception ******\n");
                Console.WriteLine(exception.Message + "\n\n\n\n\n");
            }
            return null;
        }

        /// <summary>
        /// Writes to server and reading the response so the reading will be 'pure' 
        /// and the response won't affect other processes that reads from the server 
        /// </summary>
        /// <param name="command">command to the server</param>
        public void write(string command)
        {
            try
            {
                // write the data command to the simulator
                streamWriter.WriteLine(command);
                Console.WriteLine(command);
            }
            catch (Exception exception)
            {
                //disconnect();
                //connected = false;
                throw new Exception();
            }
        }
    }
}
