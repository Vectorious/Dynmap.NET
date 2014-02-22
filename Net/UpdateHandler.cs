using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Dynmap.NET.Net
{
    internal class UpdateHandler
    {
        #region Constructors

        public UpdateHandler()
        {
            _lastUpdateTimestamp = new DateTime();
            _jsSerializer = new JavaScriptSerializer();
        }

        #endregion

        #region Events

        public event PlayerJoinEventHandler PlayerJoin;
        protected void playerJoin(string name, string account, DateTime timestamp)
        {
            if (PlayerJoin != null)
                PlayerJoin(this, new PlayerJoinQuitEventArgs { Player = new HiddenPlayer(name, account), Timestamp = timestamp });
        }

        public event PlayerQuitEventHandler PlayerQuit;
        protected void playerQuit(string name, string account, DateTime timestamp)
        {
            if (PlayerQuit != null)
                PlayerQuit(this, new PlayerJoinQuitEventArgs { Player = new HiddenPlayer(name, account), Timestamp = timestamp });
        }

        public event PlayerChatEventHandler PlayerChat;
        protected void playerChat(string name, string account, Message message)
        {
            if (PlayerChat != null)
                PlayerChat(this, new PlayerChatEventArgs { Player = new HiddenPlayer(name, account), Message = message });
        }

        #endregion

        #region Methods

        public ServerInfo ParseConfig(string input)
        {
            var config = _jsSerializer.Deserialize<dynamic>(input);

            string name = config["title"];

            var worlds = new List<string>();

            foreach (var world in config["worlds"])
                worlds.Add(world["title"]);

            return new ServerInfo(name, worlds);
        }

        public IEnumerable<Player> ParsePlayers(string input)
        {
            var updates = _jsSerializer.Deserialize<dynamic>(input);

            foreach (var player in updates["players"])
            {
                string name = player["name"];
                int armor = player["armor"];
                string account = player["account"];
                int health = player["health"];
                string type = player["type"];
                string world = player["world"];

                yield return new Player(name, armor, account, health, type, world);
            }
        }

        public void ParseUpdates(string input)
        {
            var updates = _jsSerializer.Deserialize<dynamic>(input);

            foreach (var update in updates["updates"])
            {
                var convertedTimestamp = ConvertFromJavaScriptTimestamp(update["timestamp"]);

                if (convertedTimestamp <= _lastUpdateTimestamp)
                    continue;
                
                _lastUpdateTimestamp = convertedTimestamp;

                switch ((string)update["type"])
                {
                    case "playerjoin": playerJoin(update["playerName"], update["account"], convertedTimestamp);
                        break;
                    case "playerquit": playerQuit(update["playerName"], update["account"], convertedTimestamp);
                        break;
                    case "chat": playerChat(update["playerName"], update["account"], new Message(update["message"], update["source"], convertedTimestamp));
                        break;
                }
            }                
        }

        public static DateTime ConvertFromJavaScriptTimestamp(double timestamp)
        {
            return Origin.AddMilliseconds(timestamp).ToLocalTime();
        }

        public static long ConvertToJavaScriptTimestamp(DateTime timestamp)
        {
            return (long)(timestamp.Subtract(Origin).TotalMilliseconds);
        }

        #endregion

        #region Fields

        private DateTime _lastUpdateTimestamp;
        private readonly JavaScriptSerializer _jsSerializer;

        private static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        #endregion
    }
}
