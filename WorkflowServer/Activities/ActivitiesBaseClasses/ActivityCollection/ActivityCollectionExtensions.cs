using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowServer.Activities
{
    /// <summary>
    /// Extension class for ActivityCollection that allows one to run after another
    /// </summary>
    public static class ActivityCollectionExtensions
    {
        public static IActivityCollection<IActivityContext> ConnectPipe(
            this IActivityCollection<IActivityContext> collection, IPipe pipe)
        {
            foreach (var activity in collection)
            {
                if (activity is IScanSender sender)
                {
                    sender.SendScan += pipe.SendInstructionToClient;
                }

                if (activity is IScanReceiver receiver)
                {
                    ScanQueueManager.BuildQueue(receiver.MsNOrder);
                }

            }
            return collection;
        }
    }
}
