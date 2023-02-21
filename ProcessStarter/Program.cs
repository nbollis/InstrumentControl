using System.Diagnostics;
using System.Reflection;

namespace ProcessStarter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string serverPath = args[0];
            //string clientPath = args[1];

            string serverCommands = "Server";
            string clientCommands = ". Server tribrid";

            string path = AppDomain.CurrentDomain.BaseDirectory.Split("ProcessStarter")[0];
            ProcessStartInfo serverProcessStartInfo = new(
                Path.Combine(path, @"WorkflowServer\bin\debug\net6.0\WorkflowServer.exe"))
            {
                Arguments = serverCommands
            };

            ProcessStartInfo clientProcessStartInfo = new(
                Path.Combine(path, @"InstrumentClient\bin\debug\Client.exe"))
            {
                Arguments = clientCommands
            }; 
            Process.Start(serverProcessStartInfo);
            Process.Start(clientProcessStartInfo);
            Console.ReadLine();
        }
    }
}