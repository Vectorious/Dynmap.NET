using System;

namespace Dynmap.NET.Net
{
    public class PlayerJoinQuitEventArgs : EventArgs
    {
        public IPlayer Player { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
