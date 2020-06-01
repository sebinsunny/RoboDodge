using System;
using SplashKitSDK;
using System.Collections.Generic;

public class Network
{
    public string Name { get; set; }
    private ServerSocket _server;

    private Dictionary<string, Connection> _peers = new Dictionary<string, Connection>();

    public string msg { get; private set; }

    public ServerSocket Server
    {
        get
        {
            return _server;
        }
    }

    public Network(ushort port)
    {
        _server = new ServerSocket("GameServer", port);
    }

    public void ConnectionSever(string address, ushort port)
    {
        Connection newConnection = new Connection($"{address }:{port}", address, port);
        if (newConnection.IsOpen)
        {
            Console.WriteLine($"Conected to {address}:{port}");
        }
    }

    private void EstablishConnection(Connection con)
    {
        // Send my name...
        con.SendMessage(Name);

        // Wait for a message...
        SplashKit.CheckNetworkActivity();
        for (int i = 0; i < 10 && !con.HasMessages; i++)
        {
            //SplashKit.Delay(200);
            SplashKit.CheckNetworkActivity();
        }

        if (!con.HasMessages)
        {
            con.Close();
            throw new Exception("Timeout waiting for name of peer");
        }

        // Read the name
        string name = con.ReadMessageData();

        // See if we can register this...
        if (_peers.ContainsKey(name))
        {
            con.Close();
            throw new Exception("Unable to connect to multiple peers with the same name");
        }

        // Register
        _peers[name] = con;
     
    }

    public void CheckNewConnections()
    {
        while (_server.HasNewConnections)
        {
            Connection newConnection = _server.FetchNewConnection();
            try
            {
                EstablishConnection(newConnection);
            }
            catch
            {
              
            }
        }
    }

    public void Broadcast(string message)
    {

        SplashKit.BroadcastMessage(message);
    }


    public string GetNewMessages()
    {   //Check for the network activity
        SplashKit.CheckNetworkActivity();

        while (SplashKit.HasMessages())
        {   //can extract the message using Splashkit.ReadMessage
            using (Message m = SplashKit.ReadMessage())
            {
                
                msg = m.Data;
            }
        }
        return msg;
    }
    
}
