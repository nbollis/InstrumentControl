using System;
using System.Runtime; 
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Client;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Modes;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Workflow;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;


namespace InstrumentClient
{
    public class ThermoTribrid : IInstrument
    { 
        // IScans contains CanAcceptNextCustomScan and PossibleParametersChanged. 
        public string InstrumentId { get; private set; }
        public string InstrumentName { get; private set; }
        // InstAccessContainer contains MessagesArrived, ServiceConnectionChanged. 
        public static IFusionInstrumentAccessContainer InstAccessContainer { get; private set; }
        // InstAccess contains ContactClosureChanged, AcquisitionErrorsArrived, 
        // ConnectionChanged. 
        public static IFusionInstrumentAccess InstAccess { get; private set; }
        // MsScanContainer contains events MsScanArrived
        public static IFusionMsScanContainer MsScanContainer { get; private set; }
        // InstAcq contains events AcquisitionStreamClosing, AcquisitionStreamOpening,
        // StateChanged
        public static IAcquisition InstAcq { get; private set; }
        public static IControl InstControl { get; private set; }
        // Private Properties
        private int SystemState { get; set; }
        private static IScans MScan { get; set; }
        // Constructors
        public ThermoTribrid()
        {
            InstAccessContainer = Factory<IFusionInstrumentAccessContainer>.Create();
        }
        #region OpenInstrumentConnection

        public SingleScanDataObject GetLastScan()
        {
            IMsScan scan = MsScanContainer.GetLastMsScan();
            return scan.ConvertToSingleScanDataObject(); 
        }

        public void SendScanAction(SingleScanDataObject ssdo)
        {
            if (ssdo.ScanInstructions.ScanType == null) 
                throw new ArgumentException("ScanAction type is invalid! (property was null)");
            
            //switch (ssdo.ScanInstructions.ScanType)
            ////{
            ////    case (ScanInstructions.RepeatingScan): 
            ////        SendRepeatingScan();
            ////        break;
            ////    case (ScanType.CustomScan): 
            ////        SendCustomScan();
            ////        break;
            //}
        }

        private IDictionary<string, string> SsdoToDictionary(SingleScanDataObject ssdo)
        {
            Dictionary<string, string> valuesDict = new Dictionary<string, string>();
            
            // create an ssdo property to Thermo instrument value name. 
            
            
            return valuesDict;
        }
        private IRepeatingScan CreateRepeatingScan(SingleScanDataObject ssdo)
        {
            IRepeatingScan rscan = MScan.CreateRepeatingScan();
            Dictionary<string,string> scanValues = ThermoTribridScanTranslator.TranslateSsdo(ssdo);
            
            // iterates through the scan and only sets values that differ. 
            foreach (string key in scanValues.Keys)
            {
                if (rscan.Values[key] == scanValues[key])
                {
                    continue;
                }

                rscan.Values[key] = scanValues[key];
            }

            return rscan; 

        }
        /// <summary>
        /// Uses the ScanType property of SingleScanDataObject to set a repeating scan. 
        /// </summary>
        /// <param name="ssdo"></param>
        /// <returns>Boolean value indicating if scan was sent to the instrument.</returns>
        private bool SendRepeatingScan(SingleScanDataObject ssdo)
        {
            var scan = CreateRepeatingScan(ssdo);
            return MScan.SetRepetitionScan(scan);
        }
        private ICustomScan CreateCustomScan(SingleScanDataObject ssdo)
        {
            ICustomScan cscan = MScan.CreateCustomScan();
            Dictionary<string, string> scanValues = ThermoTribridScanTranslator.TranslateSsdo(ssdo);

            // iterates through the scan and only sets values that differ. 
            foreach (string key in scanValues.Keys)
            {
                if (cscan.Values[key] == scanValues[key])
                {
                    continue;
                }

                cscan.Values[key] = scanValues[key];
            }

            return cscan;
        }

        private bool SendCustomScan(SingleScanDataObject ssdo)
        {
            ICustomScan cscan = CreateCustomScan(ssdo);
            return MScan.SetCustomScan(cscan); 
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
            MScan = InstControl.GetScans(false); 

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

            MsScanContainer.MsScanArrived += (o, s) =>
            {
                // convert to single scan data object and raise this.ScanReceived
                //
                var ssdo = s.GetScan().ConvertToSingleScanDataObject();
                EventHandler<ScanReceivedEventArgs> handler = ScanReceived;
                if (handler != null)
                {
                    handler(this, new ScanReceivedEventArgs(ssdo)); 
                }
            };

            MScan.CanAcceptNextCustomScan += (o, s) =>
            {
                EventHandler handler = InstrumentReadyToReceiveScan;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty); 
                }
            };
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

        public void GetScanPossibleParameters()
        {
            throw new NotImplementedException();
        }

        public event EventHandler InstrumentConnected;
        public event EventHandler InstrumentDisconnected;
        public event EventHandler<ScanReceivedEventArgs> ScanReceived;
        public event EventHandler InstrumentReadyToReceiveScan;
        // TODO: Change from StateChangedEventArgs to a Custom Class that doesn't use a 
        // thermo-based class. 
        public event EventHandler<StateChangedEventArgs> SystemStateChanged; 

    }

    public class ScanReceivedEventArgs : EventArgs
    {
        public SingleScanDataObject Ssdo { get; set; }
        public ScanReceivedEventArgs(SingleScanDataObject ssdo)
        {
            Ssdo = ssdo;
        }
    }

    public enum ConnectionState
    {
        Connected = 1,
        Disconnected = 0
    }
    public class InstrumentConnectionStateEventArgs : EventArgs
    {

        public ConnectionState ConnectionStatus { get; set; }

        public InstrumentConnectionStateEventArgs(ConnectionState connection)
        {
            ConnectionStatus = connection; 
        }
    }
}
