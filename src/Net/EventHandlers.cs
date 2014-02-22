namespace Dynmap.NET.Net
{
    public delegate void PlayerJoinEventHandler(object sender, PlayerJoinQuitEventArgs e);
    public delegate void PlayerQuitEventHandler(object sender, PlayerJoinQuitEventArgs e);
    public delegate void PlayerChatEventHandler(object sender, PlayerChatEventArgs e);
}
