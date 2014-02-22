namespace Dynmap.NET
{
    public class HiddenPlayer : IPlayer
    {
        #region Constructors

        public HiddenPlayer(string name, string account)
        {
            Name = name;
            Account = account;
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public string Account { get; private set; }

        #endregion
    }
}
