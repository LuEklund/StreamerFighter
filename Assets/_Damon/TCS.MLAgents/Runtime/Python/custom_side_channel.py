
import uuid
from mlagents_envs.side_channel.side_channel import SideChannel, IncomingMessage, OutgoingMessage

class StringLogSideChannel(SideChannel):
    """A custom side channel that sends and receives simple log messages."""
    def __init__(self):
        super().__init__(uuid.UUID("00000000-0000-0000-0000-000000000001"))

    def on_message_received(self, msg: IncomingMessage) -> None:
        received = msg.read_string()
        print(f"Received from Unity: {received}")

    def log(self, message: str):
        msg = OutgoingMessage()
        msg.write_string(message)
        self.queue_message_to_send(msg)
