using System.Diagnostics;

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
            ProcessStartInfo serverProcessStartInfo = new(
                @"C:\Users\Orbitrap Lumos\source\repos\InstrumentControl\ApplicationServer\bin\Debug\net6.0\ApplicationServer.exe")
            {
                Arguments = serverCommands
            };

            ProcessStartInfo clientProcessStartInfo = new(
                @"C:\Users\Orbitrap Lumos\source\repos\InstrumentControl\Client\bin\Debug\Client.exe")
            {
                Arguments = clientCommands
            }; 
            Process.Start(serverProcessStartInfo);
            Process.Start(clientProcessStartInfo);
            Console.ReadLine();
        }
    }
}