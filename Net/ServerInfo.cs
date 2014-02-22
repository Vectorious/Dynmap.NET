using System.Collections.Generic;

namespace Dynmap.NET.Net
{
    public class ServerInfo
    {
        #region Constructors

        public ServerInfo(string name, IEnumerable<string> worlds)
        {
            Name = name;
            Worlds = new List<string>(worlds);
        }

        #endregion

        #region Properties

        public string Name { get; private set; }
        public List<string> Worlds { get; private set; }

        #endregion
    }
}
