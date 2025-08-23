using Unity.MLAgents.SideChannels;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    /// <summary>
    ///     A simple side channel for sending and receiving log messages. Uses a fixed GUID so that Unity and Python match.
    /// </summary>
    public class StringLogSideChannel : SideChannel {
        public StringLogSideChannel() {
            ChannelId = new Guid( "00000000-0000-0000-0000-000000000001" );
        }

        public void Log(string message) {
            using (var msgOut = new OutgoingMessage()) {
                msgOut.WriteString( message );
                QueueMessageToSend( msgOut );
            }
        }

        protected override void OnMessageReceived(IncomingMessage msg) {
            string received = msg.ReadString();
            Debug.Log( "Received from Python: " + received );
        }
    }
}