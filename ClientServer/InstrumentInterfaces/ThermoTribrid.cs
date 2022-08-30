using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ClientServerCommunication;
using Data;
using Thermo.Interfaces.FusionAccess_V1;
using Thermo.Interfaces.FusionAccess_V1.MsScanContainer;
using Thermo.Interfaces.InstrumentAccess_V1.Control;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.TNG.Factory;
using System.IO.Pipes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;

namespace Client
{
    public class ThermoTribrid : IInstrument
    {
        public ClientPipe PipeClient { get; set; }
        public static IScans MScan { get; set; }
        public string InstrumentID { get; private set; }
        public string InstrumentName { get; private set; }
        public IFusionInstrumentAccessContainer InstAccessContainer { get; private set; }
        public IFusionInstrumentAccess InstAccess { get; private set; }
        public IFusionMsScanContainer MSScanContainer { get; private set; }
        public IAcquisition InstAcq { get; private set; }
        public IControl InstControl { get; private set; }

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
            // hold while instrument is connecting
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
            PipeClient.PipeConnected += GetInstAccess;
            // creates a ScanInstructions object and adds it to _scanQueue. 
            PipeClient.DataReceived += OnDataReceived;
            // Converts scan to SingleScanData object and invoke ReadyToSendScan method
            MSScanContainer.MsScanArrived += MsScanArrived;
            // send the SingleScanData object to the server as a byte[]. 
            ReadyToSendScan += SendScanToServer;
            while (InstAccessContainer.ServiceConnected)
            {

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
            MSScanContainer = InstAccess.GetMsScanContainer(0); 
        }

        private void MsScanArrived(object sender, MsScanEventArgs e)
        {
            // convert to SingleScanDataObject
             MsScanReadyToSendEventArgs msEventArgs = new MsScanReadyToSendEventArgs(
                 new SingleScanDataObject(e.GetScan()));
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
        public void MsScanReadyToSend(MsScanReadyToSendEventArgs eventArgs)
        { 
            ReadyToSendScan?.Invoke(this, eventArgs);
        }
    }
}
