using ClientServerCommunication;

namespace ClientServer
{
    /// <summary>
    /// Defines the common interface to all instrument types. 
    /// </summary>
    public interface IInstrument
    {
        void OpenInstrumentConnection();
        void CloseInstrumentConnection();
        void MsScanArrived();
        void EnterMainLoop();
        void SendScanToInstrument(); // scan is going to require yet another abstract factory 
        void SendScanToServer(ClientPipe clientPipe); 
    }
}