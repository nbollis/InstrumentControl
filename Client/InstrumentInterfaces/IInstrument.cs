using ClientServerCommunication;
using System;
using Data;
using Thermo.Interfaces.InstrumentAccess_V1.MsScanContainer;
using ScanInstructions = Data.ScanInstructions;

namespace InstrumentClient
{
    /// <summary>
    /// Defines the common interface to all instrument types. 
    /// </summary>
    public interface IInstrument
    {
        // get last scan
        // send action (Ms1 scan, SIM scan, boxcar scan, etc.) to instrument
        // open instrument connection
        // close instrument connection
        SingleScanDataObject GetLastScan();
        void SendScanAction(SingleScanDataObject ssdo); 
        void OpenInstrumentConnection();
        void CloseInstrumentConnection();
        string GetSystemState(int stateOrMode);
        void CancelAcquisition();
        void PauseAcquisition();
        void ResumeAcquisition();
        void StartAcquisition(string rawFileName);
        void StartMethodAcquistion(string methodFilePath, string methodName, 
            string outputFileName, string sampleName, double timeInMinutes);
        void InstrumentOn();
        void InstrumentOff();
        void InstrumentStandby(); 

        event EventHandler InstrumentConnected;
        event EventHandler InstrumentDisconnected;
        event EventHandler<EventArgs> ScanReceived;
        event EventHandler ReadyToReceiveScan; 
    }
}