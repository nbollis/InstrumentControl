using System.IO.Pipes;
using ClientServerCommLibrary;
using InstrumentControl;
using Newtonsoft;
using Newtonsoft.Json;
using WorkflowServer; 

namespace ApplicationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NamedPipeServerStream pipe = new NamedPipeServerStream("test", PipeDirection.InOut,5, 
                PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            ProcessMs1ScansDelegate ms1Del = (obj, ev) =>
            {
                //Console.WriteLine(ev.ListSsdo.First().MaxX);
            };
            ProcessMs2ScansDelegate ms2Del = (obj, ev) =>
            {
                //Console.WriteLine(ev.ListSsdo.First().PrecursorScanNumber);
            };
            AppServerPipe appPipe = new(pipe, 1, 1, ms1Del, ms2Del);

            bool connectedBool = false; 
            appPipe.PipeConnected += (obj, ev) =>
            {
                connectedBool = true; 
            };
            appPipe.PipeDataReceived += (obj, ev) =>
            {
                var scan = ev.ToSingleScanDataObject();
                Console.WriteLine(scan.MaxX);
            }; 
            appPipe.StartServer();
            while (connectedBool)
            {
                //appPipe.PipeDataReceived += (obj, ev) =>
                //{
                //    var scan = ev.ToSingleScanDataObject();
                //    Console.WriteLine(scan.MaxX.ToString());
                //}; 
            }
        }
    }
}