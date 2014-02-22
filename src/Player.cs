using System;

namespace Dynmap.NET
{
    public class Player : IPlayer, IEquatable<Player>
    {
        #region Constructors

        public Player(string name, int armor, string account, int health, string type, string world)
        {
            Name = name;
            Armor = armor;
            Account = account;
            Health = health;
            Type = type;
            World = world;
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public int Armor { get; set; }
        public string Account { get; private set; }
        public int Health { get; set; }
        public string Type { get; set; }
        public string World { get; set; }

        #endregion

        #region Operators

        public static bool operator ==(Player p1, Player p2)
        {
            return p1 != null && p1.Equals(p2);
        }

        public static bool operator !=(Player p1, Player p2)
        {
            return p1 != null && !p1.Equals(p2);
        }

        #endregion

        #region IEquatable<Player> Methods

        public bool Equals(Player other)
        {
            return Account == other.Account;
        }

        public override bool Equals(object obj)
        {
            return (obj is Player) && Equals(obj);
        }

        public override int GetHashCode()
        {
            return Account.GetHashCode();
        }

        #endregion
    }
}
