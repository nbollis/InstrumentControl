using System;
using ClientServerCommLibrary;

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
        void GetSystemState();
        void CancelAcquisition();
        void PauseAcquisition();
        void ResumeAcquisition();
        void StartAcquisition();
        void StartMethodAcquistion(string methodFilePath, string methodName, 
            string outputFileName, string sampleName, double timeInMinutes);
        void InstrumentOn();
        void InstrumentOff();
        void InstrumentStandby();
        void MainLoop();
        void ResetToBaseScan(); 
        event EventHandler InstrumentConnected;
        event EventHandler InstrumentDisconnected;
        event EventHandler<MsScanArrivedEventArgs> ScanReceived;
        event EventHandler ReadyToReceiveScanInstructions; 
    }
}