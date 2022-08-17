using ClientServerCommunication;
namespace ClientInstrument
{
    /// <summary>
    /// Defines the common interface to all instrument types. 
    /// </summary>
    public interface IInstrument
    {
        void OpenInstrumentConnection();
        void CloseInstrumentConnection();
        void MsScanArrived();
        /// <summary>
        /// Begins the main loop of the instrument api. This method should contain all events within the
        /// api and their corresponding listeners. 
        /// </summary>
        void EnterMainLoop();
        /// <summary>
        /// The body of this method should convert the ScanInstructions object to something that is consumable
        /// by the instrument's own API. 
        /// </summary>
        void SendScanToInstrument(ScanInstructions scanInstructions); // scan is going to require yet another abstract factory 
        /// <summary>
        /// Adds the communication to the server running the workflow.
        /// </summary>
        /// <param name="clientPipe"></param>
        void AddClientPipe(ClientPipe clientPipe); 
    }
}