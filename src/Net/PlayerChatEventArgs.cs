using System;

namespace Dynmap.NET.Net
{
    public class PlayerChatEventArgs : EventArgs
    {
        public IPlayer Player { get; set; }
        public DateTime Timestamp { get; set; }
        public Message Message { get; set; }
    }
}
