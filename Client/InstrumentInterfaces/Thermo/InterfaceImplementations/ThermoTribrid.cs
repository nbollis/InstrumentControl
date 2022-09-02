using System;
using System.Runtime; 
using System.Collections.Generic;
using System.Threading;
using ClientServerCommunication;
using Thermo.Interfaces.FusionAccess_V1;
using Thermo.Interfaces.FusionAccess_V1.MsScanContainer;
using Thermo.Interfaces.InstrumentAccess_V1.Control;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.TNG.Factory;
using System.Linq;
using Data; 
using System.Threading.Tasks;
using Newtonsoft.Json;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;
using ScanInstructions = ClientServerCommunication.ScanInstructions;


namespace Client
{
    public class ThermoTribrid : IInstrument
    {
        public ClientPipe PipeClient { get; set; }
        public static IScans MScan { get; set; }
        public string InstrumentID { get; private set; }
        public string InstrumentName { get; private set; }
        public static IFusionInstrumentAccessContainer InstAccessContainer { get; private set; }
        public static IFusionInstrumentAccess InstAccess { get; private set; }
        public static IFusionMsScanContainer MsScanContainer { get; private set; }
        public static IAcquisition InstAcq { get; private set; }
        public static IControl InstControl { get; private set; }

        public event EventHandler<MsScanReadyToSendEventArgs> ReadyToSendScan;
        
        private Queue<ScanInstructions> _scanQueue = new Queue<ScanInstructions>();
        public event EventHandler<ScanInstructionsEventArgs> ScanInstructionReceived; 

        public ThermoTribrid()
        {
            InstAccessContainer = Factory<IFusionInstrumentAccessContainer>.Create(); 
        }
        public void OpenInstrumentConnection()
        {
            InstAccessContainer.StartOnlineAccess();
            while (!InstAccessContainer.ServiceConnected) ;
            GetInstAccess();
            Console.WriteLine("Instrument access started"); 
        }

        public void CloseInstrumentConnection()
        {
            InstAccessContainer.Dispose();
        }

        public void EnterMainLoop()
        {
            // connect to Server 
            PipeClient.Connect();
            // initializes instrument access. 
            // PipeClient.PipeConnected += GetInstAccess;
            PipeClient.DataReceived += OnDataReceived;
            // Converts scan to SingleScanData object and invoke ReadyToSendScan method
            MsScanContainer.MsScanArrived += MsScanArrived; 
            // send the SingleScanData object to the server as a byte[]. 
            ReadyToSendScan += SendScanToServer;

            while (InstAccessContainer.ServiceConnected)
            { 
                // creates a ScanInstructions object and adds it to _scanQueue. 

            }
            PipeClient.Close();
        }
        private void OnDataReceived(object sender, PipeEventArgs args)
        {
            // TODO: Convert to concurrent queue. 
            // to prevent multiple writes to the queue in the event 
            // of multiple ScanInstructionObjects getting sent in the buffer rapidly.
            // enqueue ScanInstructionObject
            _scanQueue.Enqueue(PipeClient
                .DeserializeByteStream<ScanInstructions>(args.Data));
            
            // The listener for CanAcceptNextCustomScan 
            // Creates a delegate to receive the EventArgs from the CanAcceptNextCustomScan event. 
            
            // TODO: Refactor to pull out the AutoResentEvent and EventHandler from this method. 
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            EventHandler ev = delegate(object o, EventArgs e)
            {
                waitHandle.Set();
            };
            // Delegate listens for the CanAcceptNextCustomScan
            MScan.CanAcceptNextCustomScan += ev;
            Task.Run(() =>
                {
                    // TODO: add conversion from ScanInstructions to ICustomScan. 
                    ICustomScan scan = MScan.CreateCustomScan();
                    MScan.SetCustomScan(scan);
                }
            );
            // Blocks thread from continuing until it receives the CanAcceptNextCustomScanEvent. 
            waitHandle.WaitOne();
        }
        private void GetInstAccess(object sender, EventArgs args)
        {
            InstAccess = InstAccessContainer.Get(1);
            // do not change order. InstAccess must be filled first as the other
            // properties depend on it to be filled themselves.
            InstControl = InstAccess.Control;
            InstAcq = InstControl.Acquisition;
            InstrumentID = InstAccess.InstrumentId.ToString();
            InstrumentName = InstAccess.InstrumentName;
            MsScanContainer = InstAccess.GetMsScanContainer(0); 
        }
        private void GetInstAccess()
        {
            InstAccess = InstAccessContainer.Get(1);
            // do not change order. InstAccess must be filled first as the other
            // properties depend on it to be filled themselves.
            InstControl = InstAccess.Control;
            InstAcq = InstControl.Acquisition;
            InstrumentID = InstAccess.InstrumentId.ToString();
            InstrumentName = InstAccess.InstrumentName;
            MsScanContainer = InstAccess.GetMsScanContainer(0);
        }

        private void MsScanArrived(object sender, MsScanEventArgs e)
        {

            // convert to SingleScanDataObject
             MsScanReadyToSendEventArgs msEventArgs = 
                 new MsScanReadyToSendEventArgs(e.GetScan().ConvertToSingleScanDataObject());
            Console.WriteLine("OnDataReceived MS Scan Number: {0}", msEventArgs.ScanData.MinX.ToString()); 
             // raise ready to send event
             MsScanReadyToSend(msEventArgs);
        }
        public void SendScanInstructionsToInstrument()
        {
            
        }

        public void SendScanToServer(object sender, MsScanReadyToSendEventArgs eventArgs)
        {
            string str = JsonConvert.SerializeObject(eventArgs.ScanData);
            PipeClient.WriteString(str); 
        }
        // General note: in .NET Framework 4.8, event handling needs to be done like the below format, 
        // not the handler?.Invoke(this, eventArgs) like in .NET 6.0 
        public void MsScanReadyToSend(MsScanReadyToSendEventArgs eventArgs)
        {
            EventHandler<MsScanReadyToSendEventArgs> handler = ReadyToSendScan;
            if (handler != null)
            {
                handler(this, eventArgs); 
            }
        }


    }
}
