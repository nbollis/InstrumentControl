using ClientServerCommLibrary;
using System;

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
        
        /// <summary>
        /// Used for explicitly gaining access to the last received instrument scan without requiring an event. 
        /// </summary>
        /// <returns>Returns SingleScanDataObject. </returns>
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
        void GetScanPossibleParameters();

        event EventHandler InstrumentConnected;
        event EventHandler InstrumentDisconnected;
        event EventHandler<ScanReceivedEventArgs> ScanReceived;
        event EventHandler InstrumentReadyToReceiveScan;
    }
}