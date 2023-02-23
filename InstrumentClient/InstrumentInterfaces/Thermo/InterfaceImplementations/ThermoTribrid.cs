﻿using System;
using System.Runtime; 
using System.Collections.Generic;
using System.Threading;
using ClientServerCommLibrary;
using Thermo.Interfaces.FusionAccess_V1;
using Thermo.Interfaces.FusionAccess_V1.MsScanContainer;
using Thermo.Interfaces.InstrumentAccess_V1.Control;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition;
using Thermo.TNG.Factory;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Modes;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Acquisition.Workflow;
using Thermo.Interfaces.InstrumentAccess_V1.Control.Scans;
using Thermo.Interfaces.FusionAccess_V1.Control; 
using System.Linq;
using System.Threading.Tasks; 


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
        public static IFusionControl _fusionControl { get; private set; }
        // Private Properties
        private int SystemState { get; set; }
        IScans scan { get; set; }
        ICustomScan customScan { get; set; }
        IRepeatingScan repeatingScan { get; set; }

        public event EventHandler InstrumentConnected;
        public event EventHandler InstrumentDisconnected;
        public event EventHandler<MsScanArrivedEventArgs> ScanReceived;
        public event EventHandler ReadyToReceiveScanInstructions;
        public event EventHandler<EventArgs> InstrumentStateChanged;

        // Constructors
        public ThermoTribrid()
        {
            InstAccessContainer = Factory<IFusionInstrumentAccessContainer>.Create();
            InstAccessContainer.StartOnlineAccess();

            while (!InstAccessContainer.ServiceConnected) ;

            InstAccess = InstAccessContainer.Get(1);
            // do not change order. InstAccess must be filled first as the other
            // properties depend on it to be filled themselves.
            InstControl = InstAccess.Control;
            _fusionControl = (IFusionControl)InstAccess.Control; 
            
            InstAcq = InstControl.Acquisition;
            InstrumentId = InstAccess.InstrumentId.ToString();
            InstrumentName = InstAccess.InstrumentName;
            MsScanContainer = InstAccess.GetMsScanContainer(0);
            scan = InstControl.GetScans(true);
            PrintoutMessage.Print(MessageSource.Instrument, "Instrument access started");
            GetInstAccess();
            //Neither of these events work on the fusion lumos.
            scan.CanAcceptNextCustomScan += (o, e) =>
            {
                EventHandler handler = ReadyToReceiveScanInstructions;
                if (handler != null)
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
            };
            // scan.PossibleParametersChanged += (o, e) =>
            // {
            //     PrintoutMessage.Print(MessageSource.Instrument, "Possible Parameters Changed Event Fired");
            // };
            InstAccessContainer.ServiceConnectionChanged += (o, s) => { };
            InstAccessContainer.MessagesArrived += (o, s) => { };
            InstAcq.AcquisitionStreamClosing += (o, s) => { };
            InstAcq.AcquisitionStreamOpening += (o, s) => { };


            InstAcq.StateChanged += (o, s) => {
                EventHandler<EventArgs> handler = InstrumentStateChanged;
                if (handler != null)
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
            };

            // instacq systemstate also contains an enum, where each value corresponds to the acquisition state 
            // of the system. Could potentially use this as a read-back for the client. 
            // InstAcq.State.SystemState
            // The pattern in the lines below passes the events received from the instrument up the 
            // "chain of command." When operating in "smart control" mode, instrument handling and events need 
            // to be hanlded by the "brains", whether that is the app or the instrument client interface. 
            MsScanContainer.MsScanArrived += OnScanArrived;
        }
        #region OpenInstrumentConnection

        public SingleScanDataObject GetLastScan()
        {
            throw new NotImplementedException();
        }

        public void SendScanAction(SingleScanDataObject ssdo)
        {
            IDictionary<string, string> dict = ssdo.ScanInstructions.ToThermoTribridCompatibleDictionary();
            // convert the SSDO to a custom scan object. 
            
            if (ssdo.ScanInstructions.CustomOrRepeating == CustomOrRepeatingScan.Custom)
            {
                customScan = scan.CreateCustomScan();
                customScan.SingleProcessingDelay = 0; 
                
                
                foreach (var kvp in dict)
                {
                    customScan.Values[kvp.Key] = kvp.Value; 
                }

                if (scan.SetCustomScan(customScan))
                {
                    PrintoutMessage.Print(MessageSource.Instrument, "Instructions sent to instrument."); 
                }
                customScan = null; 
            }
            else if(ssdo.ScanInstructions.CustomOrRepeating == CustomOrRepeatingScan.Repeating)
            {
                repeatingScan = scan.CreateRepeatingScan();
                foreach(var kvp in dict)
                {
                    repeatingScan.Values[kvp.Key] = kvp.Value; 
                }
                bool sentToInstrument = scan.SetRepetitionScan(repeatingScan);
                EventHandler handler = ReadyToReceiveScanInstructions;
                if (handler != null)
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
                repeatingScan = null; 
            }
            else
            {
                throw new ArgumentException("ScanInstructions object no CustomOrRepeatingScan enum"); 
            }            
        }
        public void ResetToBaseScan()
        {
            scan.CancelCustomScan();
        }
        public void OpenInstrumentConnection()
        {

        }
        public void MainLoop()
        {

        }
        private void GetInstAccess()
        {
            
        }
        #endregion
        private void OnScanArrived(object o, Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer.MsScanEventArgs s)
        {
            var scan = s.GetScan();
            bool orderBool = scan.Header.TryGetValue("MSOrder", out string value);

            int order = 0;
            double precursorMz = 0;
            int scanNumber = 0;
            int precursorScanNumber = 0;

            bool scanbool = scan.Header.TryGetValue("Scan", out string scanNumberString);
            if (scanbool)
            {
                scanNumber = int.Parse(scanNumberString);
            }

            if (orderBool)
            {
                order = int.Parse(value);
                if (order > 1)
                {
                    bool precursorBool = scan.Header.TryGetValue("PrecursorMass[0]", out string precursorString);
                    if (precursorBool)
                    {
                        precursorMz = double.Parse(precursorString);
                    }
                }
            }

            var ssdo = new SingleScanDataObject()
            {
                ScanInstructions = null,
                MsNOrder = order,
                ScanNumber = scanNumber,
                MzPrecursor = precursorMz,
                XArray = scan.Centroids.Select(i => i.Mz).ToArray(),
                YArray = scan.Centroids.Select(i => i.Intensity).ToArray()
            };

            EventHandler<MsScanArrivedEventArgs> handler = ScanReceived;
            if (handler != null)
            {
                handler.Invoke(this, new MsScanArrivedEventArgs(ssdo));
            }
        
        }
        #region

        public void GetSystemState()
        {
            
        }

        public void CancelAcquisition()
        {
            throw new NotImplementedException();
        }

        public void PauseAcquisition()
        {
            throw new NotImplementedException();
        }

        public void ResumeAcquisition()
        {
            throw new NotImplementedException();
        }

        public void StartAcquisition()
        {
            throw new NotImplementedException();
        }

        public void StartMethodAcquistion(string methodFilePath, string methodName, string outputFileName, string sampleName,
            double timeInMinutes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CloseInstrumentConnection
        public void CloseInstrumentConnection()
        {
            InstAccessContainer.Dispose();
        }
        #endregion
        // enter main loop is obsolete because of the client pipe restructuring. 
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
        public void StartMethodAcquisition(string methodFilePath, string methodName,
            string outputFileName, string sampleName, double timeInMinutes)
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



    }
}
