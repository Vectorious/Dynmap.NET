Dynmap.NET
==========

A .NET library for interacting with [Dynmap](https://github.com/webbukkit/dynmap) chat. 

Usage
==========

The following code would set up a connection to a server, hook up an event for chat messages, start listening for chat messages, and send a message to the server.

```csharp
var connection = new ServerConnection("http://yourserver.com:8123/");
connection.PlayerChat += connection_PlayerChat;
connection.Start();
connection.SendMessage("Hello, world!");
```
