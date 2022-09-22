using System;
using System.Runtime; 
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ClientServerCommLibrary;
using Thermo.Interfaces.FusionAccess_V1;
using Thermo.Interfaces.FusionAccess_V1.MsScanContainer;
using Thermo.Interfaces.InstrumentAccess_V1.Control;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using Thermo.TNG.Factory;
using System.Linq;
using System.Text;
using Data; 
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Modes;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Workflow;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;

using ScanInstructions = ClientServerCommunication.ScanInstructions;
using SingleScanDataObject = Data.SingleScanDataObject;


namespace InstrumentClient
{
    public class ThermoTribrid : IInstrument
    {
        public ClientPipe PipeClient { get; set; }
        public static IScans MScan { get; set; }
        public string InstrumentId { get; private set; }
        public string InstrumentName { get; private set; }
        public static IFusionInstrumentAccessContainer InstAccessContainer { get; private set; }
        public static IFusionInstrumentAccess InstAccess { get; private set; }
        public static IFusionMsScanContainer MsScanContainer { get; private set; }
        public static IAcquisition InstAcq { get; private set; }
        public static IControl InstControl { get; private set; }
        // Private Properties
        private int SystemState { get; set; }
        // Constructors
        public ThermoTribrid()
        {
            InstAccessContainer = Factory<IFusionInstrumentAccessContainer>.Create(); 
        }
        #region OpenInstrumentConnection

        public SingleScanDataObject GetLastScan()
        {
            throw new NotImplementedException();
        }

        public void SendScanAction(SingleScanDataObject ssdo)
        {
            throw new NotImplementedException();
        }

        public void OpenInstrumentConnection()
        {
            InstAccessContainer.StartOnlineAccess();
            
            while (!InstAccessContainer.ServiceConnected) ;
            GetInstAccess();
            Console.WriteLine("Instrument access started"); 
        }
        private void GetInstAccess()
        {
            InstAccess = InstAccessContainer.Get(1);
            // do not change order. InstAccess must be filled first as the other
            // properties depend on it to be filled themselves.
            InstControl = InstAccess.Control;
            InstAcq = InstControl.Acquisition;
            InstrumentId = InstAccess.InstrumentId.ToString();
            InstrumentName = InstAccess.InstrumentName;
            MsScanContainer = InstAccess.GetMsScanContainer(0);

            InstAccessContainer.ServiceConnectionChanged += (o, s) => { };
            InstAccessContainer.MessagesArrived += (o, s) => { };
            InstAcq.AcquisitionStreamClosing += (o, s) => { };
            InstAcq.AcquisitionStreamOpening += (o, s) => { };
            InstAcq.StateChanged += (o, s) =>
            {
                EventHandler<StateChangedEventArgs> handler = SystemStateChanged;
                if (handler != null)
                {
                    handler(this, s); 
                }
            };
            // instacq systemstate also contains an enum, where each value corresponds to the acquisition state 
            // of the system. Could potentially use this as a read-back for the client. 
            // InstAcq.State.SystemState
            MsScanContainer.MsScanArrived += (o, s) => { };
            
        }
        #endregion
        #region

        public string GetSystemState(int stateOrMode)
        {
            if (stateOrMode > 2 || stateOrMode < 0)
                throw new ArgumentException("Integer selection is outside of bounds."); 

            switch (stateOrMode)
            {
                case 0:
                    return Enum.GetName(typeof(InstrumentState), InstAcq.State.SystemState);
                case 1:
                    return Enum.GetName(typeof(SystemMode), InstAcq.State.SystemMode);
                case 2:
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(Enum.GetName(typeof(InstrumentState), InstAcq.State.SystemState)); 
                    sb.AppendLine(Enum.GetName(typeof(SystemMode), InstAcq.State.SystemMode));
                    return sb.ToString();
                };
            }

            throw new ArgumentException("Error: State and Mode unable to be found."); 
        }

        public void CancelAcquisition()
        {
            InstAcq.CancelAcquisition();
        }

        public void PauseAcquisition()
        {
            if (InstAcq.CanPause)
            {
                InstAcq.Pause();
            }
            // todo: add handling for the case where InstAcq.CanPause is false. 
        }

        public void ResumeAcquisition()
        {
            if (InstAcq.CanResume)
            {
                InstAcq.Resume();
            }
            // todo: add handling for case where CanResume == false. 
        }

        public void StartAcquisition(string rawFileName)
        {
            var acquisition = CreateTuneAcquisition(rawFileName);
            InstAcq.StartAcquisition(acquisition); 
        }

        private IAcquisitionWorkflow CreateTuneAcquisition(string rawFileName)
        {
            var acquisition = InstAcq.CreatePermanentAcquisition();
            if (rawFileName != null) acquisition.RawFileName = rawFileName;
            return acquisition; 
        }

        public void StartMethodAcquistion(string methodFilePath, string methodName, string outputFileName, string sampleName,
            double timeInMinutes)
        {
            var method = CreateMethodAcquisition(methodFilePath, 5, AcquisitionContinuation.Standby,
                waitForContactClosure: true, methodName, outputFileName, sampleName, timeInMinutes);
            InstAcq.StartAcquisition(method);
        }
        private IAcquisitionMethodRun CreateMethodAcquisition(string methodFilePath,
            int singleProcessingDelay, AcquisitionContinuation continuation,
            bool waitForContactClosure, string methodName,
            string rawFileName, string sampleName,
            double timeInMinutes)
        {
            var methodAcquisition = InstAcq.CreateMethodAcquisition(methodFilePath);
            // note: you can set the single processing delay. The instrument will wait 
            // the number of milliseconds you set before it starts the next scan. 
            // this needs to be set in the implementation of the interface because I'm not sure if
            // anything besides Thermo will have this setting. 
            methodAcquisition.SingleProcessingDelay = singleProcessingDelay;
            // set the default behavior of inter-acquisition time to put the instrument on standby. 
            methodAcquisition.Continuation = continuation;
            methodAcquisition.WaitForContactClosure = waitForContactClosure;
            methodAcquisition.MethodName = methodName;
            methodAcquisition.RawFileName = rawFileName;
            methodAcquisition.SampleName = sampleName;
            methodAcquisition.Duration = TimeSpan.FromMinutes(timeInMinutes);
            return methodAcquisition;
        }

        #endregion

        #region CloseInstrumentConnection
        public void CloseInstrumentConnection()
        {
            InstAccessContainer.Dispose();
        }
        #endregion
        
        #region SendScanAction

        #endregion

        #region GetLastScan

        #endregion
        
        public void InstrumentOn()
        {
            var onMode = InstAcq.CreateOnMode();
            InstAcq.SetMode(onMode); 
        }
        public void InstrumentOff()
        {
            IOffMode offMode = InstAcq.CreateOffMode();
            InstAcq.SetMode(offMode);
        }

        public void InstrumentStandby()
        {
            IStandbyMode sbMode = InstAcq.CreateStandbyMode();
            InstAcq.SetMode(sbMode);
        }

        public event EventHandler InstrumentConnected;
        public event EventHandler InstrumentDisconnected;
        public event EventHandler<EventArgs> ScanReceived;
        public event EventHandler ReadyToReceiveScan;
        // TODO: Change from StateChangedEventArgs to a Custom Class that doesn't use a 
        // thermo-based class. 
        public event EventHandler<StateChangedEventArgs> SystemStateChanged; 

    }
}
