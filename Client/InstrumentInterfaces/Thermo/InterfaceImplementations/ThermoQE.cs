using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerCommLibrary;


namespace InstrumentClient
{
    internal class ThermoQE : IInstrument
    {
        public ClientPipe PipeClient { get; set; }

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
            throw new System.NotSupportedException();
        }

        public void CloseInstrumentConnection()
        {
            throw new NotSupportedException(); 
        }

        public string GetSystemState(int stateOrMode)
        {
            throw new NotImplementedException();
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

        public void StartAcquisition(string rawFileName)
        {
            throw new NotImplementedException();
        }

        public void StartMethodAcquistion(string methodFilePath, string methodName, string outputFileName, string sampleName,
            double timeInMinutes)
        {
            throw new NotImplementedException();
        }

        public void InstrumentOn()
        {
            throw new NotImplementedException();
        }

        public void InstrumentOff()
        {
            throw new NotImplementedException();
        }

        public void InstrumentStandby()
        {
            throw new NotImplementedException();
        }

        public void GetScanPossibleParameters()
        {
            throw new NotImplementedException();
        }

        public event EventHandler InstrumentConnected;
        public event EventHandler InstrumentDisconnected;
        public event EventHandler<ScanReceivedEventArgs> ScanReceived;
        public event EventHandler InstrumentReadyToReceiveScan;

        public void MsScanReadyToSend(MsScanReadyToSendEventArgs scanEventArgs)
        {
            throw new NotImplementedException();
        }

        public void EnterMainLoop()
        {
            throw new NotImplementedException();
        }

        public void SendScanInstructionsToInstrument()
        {
            throw new NotImplementedException();
        }

        public void SendScanToServer(object sender, MsScanReadyToSendEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
