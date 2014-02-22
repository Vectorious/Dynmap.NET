using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using System.Web.Script.Serialization;

namespace Dynmap.NET.Net
{
    public class ServerConnection : IDisposable
    {
        #region Constructors

        public ServerConnection(string uri, int updateInterval = 2000)
        {
            _started = false;
            _uri = new Uri(uri);
            UpdateInterval = updateInterval;

            _playerDictionary = new Dictionary<string, Player>();
            _updateHandler = new UpdateHandler();

            _updateHandler.PlayerJoin += _updateHandler_PlayerJoin;
            _updateHandler.PlayerQuit += _updateHandler_PlayerQuit;
            _updateHandler.PlayerChat += _updateHandler_PlayerChat;

            _timer = new Timer();

            _timer.Elapsed += _timer_Elapsed;

            using (var client = new WebClient())
                _serverInfo = _updateHandler.ParseConfig(client.DownloadString(new Uri(_uri, "up/configuration")));

        }

        #endregion

        #region Events

        public event PlayerJoinEventHandler PlayerJoin;
        protected void playerJoin(IPlayer player, DateTime timestamp)
        {
            if (PlayerJoin != null)
                PlayerJoin(this, new PlayerJoinQuitEventArgs { Player = player, Timestamp = timestamp });
        }

        public event PlayerQuitEventHandler PlayerQuit;
        protected void playerQuit(IPlayer player, DateTime timestamp)
        {
            if (PlayerQuit != null)
                PlayerQuit(this, new PlayerJoinQuitEventArgs { Player = player, Timestamp = timestamp });
        }

        public event PlayerChatEventHandler PlayerChat;
        protected void playerChat(IPlayer player, Message message)
        {
            if (PlayerChat != null)
                PlayerChat(this, new PlayerChatEventArgs { Player = player, Message = message });
        }

        #endregion

        #region Methods

        public void Start()
        {
            if (_started)
                return;

            _started = true;

            Update();

            _timer.Interval = UpdateInterval;

            _timer.Start();
        }

        public void Stop()
        {
            if (!_started)
                return;

            _started = false;
            _timer.Stop();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        public void SendMessage(string text)
        {
            string json = new JavaScriptSerializer().Serialize(new
                {
                    name = "",
                    message = text
                });

            using (var client = new WebClient())
                client.UploadString(new Uri(_uri, "up/sendmessage"), json);
        }

        public void UpdatePlayers()
        {
            long timestamp = GetTimestamp();

            string json;

            using (var client = new WebClient())
                json = client.DownloadString(new Uri(_uri, string.Format("up/world/{0}/{1}", _serverInfo.Worlds[0], timestamp)));

            UpdatePlayers(json);
        }

        private void UpdatePlayers(string json)
        {
            var players = _updateHandler.ParsePlayers(json);

            foreach (Player player in players)
            {
                if (!_playerDictionary.ContainsKey(player.Account))
                    _playerDictionary.Add(player.Account, player);
                else
                    _playerDictionary[player.Account] = player;
            }
        }

        private static long GetTimestamp()
        {
            return UpdateHandler.ConvertToJavaScriptTimestamp(DateTime.UtcNow);
        }

        private void Update()
        {
            long timestamp = GetTimestamp();

            string json;

            using (var client = new WebClient())
                json = client.DownloadString(new Uri(_uri, string.Format("up/world/{0}/{1}", _serverInfo.Worlds[0], timestamp)));

            UpdatePlayers(json);

            _updateHandler.ParseUpdates(json);
        }

        private IPlayer ResolvePlayer(IPlayer player)
        {
            if (player.Account == null)
                return new HiddenPlayer(player.Name, "(Hidden)");
            return _playerDictionary.ContainsKey(player.Account) ? _playerDictionary[player.Account] : player;
        }

        private void RemovePlayer(IPlayer player)
        {
            if (_playerDictionary.ContainsKey(player.Account))
                _playerDictionary.Remove(player.Account);
        }

        void _updateHandler_PlayerJoin(object sender, PlayerJoinQuitEventArgs e)
        {
            playerJoin(ResolvePlayer(e.Player), e.Timestamp);
        }

        void _updateHandler_PlayerQuit(object sender, PlayerJoinQuitEventArgs e)
        {
            playerQuit(ResolvePlayer(e.Player), e.Timestamp);
            RemovePlayer(e.Player);
        }

        void _updateHandler_PlayerChat(object sender, PlayerChatEventArgs e)
        {
            playerChat(ResolvePlayer(e.Player), e.Message);
        }

        #endregion

        #region Properties

        public IEnumerable<Player> Players { get { return _playerDictionary.Values.AsEnumerable(); } }

        public int UpdateInterval { get; set; } 

        #endregion

        #region Fields

        private readonly UpdateHandler _updateHandler;
        private readonly Timer _timer;

        private readonly Dictionary<string, Player> _playerDictionary;
        private ServerInfo _serverInfo;
        private readonly Uri _uri;
        private bool _started;

        #endregion

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }

            _updateHandler.PlayerJoin -= _updateHandler_PlayerJoin;
            _updateHandler.PlayerQuit -= _updateHandler_PlayerQuit;
            _updateHandler.PlayerChat -= _updateHandler_PlayerChat;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
