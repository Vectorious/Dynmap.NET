using System;

namespace Dynmap.NET
{
    public class Message
    {
        #region Constructors

        public Message(string text, string source, DateTime timestamp)
        {
            Text = text;
            Source = source;
            Timestamp = timestamp;
        }

        #endregion

        #region Properties

        public string Text { get; private set; }
        public string Source { get; private set; }
        public DateTime Timestamp { get; private set; }

        #endregion
    }
}
