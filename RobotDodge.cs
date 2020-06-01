using System;
using System.Collections.Generic;
using SplashKitSDK;

public class RobotDodge
{
    private Player _Player;
    private Window _GameWindow;
    private List<Robot> Robots = new List<Robot>();
    private List<Bullet> Bullets = new List<Bullet>();
    private List<NetworkPlayer> _networkPlayer = new List<NetworkPlayer>();
    public bool OnlineGame { get; private set; }
    public bool IsServer { get; private set; }
    public Network boardcast{ get; private set; }
    private List<string> _otherNetworkNames = new List<string>();
    private string _otherPlayerMsg;
    public Timer myTimer;
    private string name = "";
    private double x = 0, y = 0, radius = 0;
    private int score = 0;
    public bool Quit
    {
        get { return _Player.Quit; }
    }

    public RobotDodge(Window gameWindow)
    {
        
        string answer;
        Console.Write("What is your name: ");
        string name = Console.ReadLine();

        do
        {
            Console.Write("Do you want to play it online? (Y/N) ");
            answer = Console.ReadLine();
        }
        while (answer.ToUpper() != "Y" && answer.ToUpper() != "N");

        if (answer.ToUpper() == "N") // Not online game, offline game
        {
            OnlineGame = false;
        }
        else if (answer.ToUpper() == "Y") // Online game
        {
            OnlineGame = true;
            Console.Write("Which port to run at: ");
            ushort port = Convert.ToUInt16(Console.ReadLine());
            Network ping = new Network(port) { Name = name };
            boardcast = ping;

            string isHost;
            do
            {
                Console.Write("Is this the host? (Y/N) ");
                isHost = Console.ReadLine();
            } while (isHost.ToUpper() != "Y" && isHost.ToUpper() != "N");

            if (isHost.ToUpper() == "N") // Not host server, select server to connect to
            {
                IsServer = false;
                MakeNewConnection(ping);
            }
            else if (isHost.ToUpper() == "Y") // Be the host server
            {
                IsServer = true;
            }
        }
        
        _GameWindow = gameWindow;
        _Player = new Player(gameWindow,name);
        RandomRobot();
        myTimer = new Timer("Timer");
        myTimer.Start();

    }

    public void HandleInput()
    {
        _Player.HandleInput();
        _Player.StayOnWindow(_GameWindow);
        if (_Player.fire == true) {
            _Player.fire = false;
            Bullets.Add (RandomBullet());
        }
    }

    public void Draw()
    {
   _GameWindow.Clear(Color.LightYellow);
        _Player.Score = Convert.ToInt32(myTimer.Ticks / 1000);
        foreach (Robot robot in Robots)
        {
            robot.Draw();
        }
        _Player.Draw();
        
        
        if (_networkPlayer.Count > 0)
                {
                    foreach (NetworkPlayer op in _networkPlayer)
                    {
                        op.Draw();
                    }
                }
        
        
        
        
        if (Bullets != null)
        {
            foreach (Bullet bullet in Bullets)
            {
                bullet.Draw();
            }
        }
         
        
       
        _GameWindow.DrawText ($"{_Player.Score}", Color.Black, "BoldFont", 35, 750, 30);
        _GameWindow.Refresh(60);
    }

    public void Update()
    {
        //add random number of robots into the list
        if (SplashKit.Rnd (100) < 2) Robots.Add (RandomRobot ());
       
        foreach (Robot robot in Robots)
        {
            robot.Update();
        }
        if (Bullets != null) {
            foreach (Bullet bullet in Bullets) {
                bullet.Update ();
            }
        }
        
        if (OnlineGame)
        {
            UpdateInfo();
            playerinfo();
        }

       
        
        CheckCollisions();
    }
    private void CheckCollisions()
    {   List<Robot> removeRobots = new List<Robot> ();
        List<Bullet> removeBullets = new List<Bullet> ();
        
        foreach (NetworkPlayer n in _networkPlayer)
        {
            if (_Player.CollidedWith(n))
            {
                if (_Player.Radius <  n.Radius)
                {
                    _Player.Score++;
                }
                else if (_Player.Radius > n.Radius)
                {
                    _Player.Score--;
                }
              
                
                
            }
        }
        
        

        foreach (Robot robot in Robots)
        {
//check the player and robot collision to remove the robot from main list
            if (_Player.CollidedWith(robot) || robot.IsOffscreen(_GameWindow))
            {
                removeRobots.Add(robot);
                if (_Player.CollidedWith (robot)) {
                    _Player.Lives -= 1;
                    if (_Player.Lives <= 0) _Player.Quit = true;
                }
            }
            if(Bullets != null) {
                foreach (Bullet bullet in Bullets) {
                    if (bullet.BulletCollidedWith(robot)) {
                        removeRobots.Add (robot);
                        removeBullets.Add (bullet);
                    }
                    if (bullet.IsOffscreen(_GameWindow)) removeBullets.Add (bullet);
                }
                
                
                
            }
        }
        foreach (Robot robot in removeRobots) {
            Robots.Remove (robot);
        }
        foreach (Bullet bullet in removeBullets) {
            Bullets.Remove (bullet);
        }  
             
    }
    
    
    

    public Robot RandomRobot()
    {
        
        Robot _TestRobot;
        if (SplashKit.Rnd () < 0.5) {
            _TestRobot = new Boxy (_GameWindow, _Player);
        } else {
            if (SplashKit.Rnd() < 0.5) {
                _TestRobot = new Roundy (_GameWindow, _Player);
            }
            else {
                _TestRobot = new Octopus (_GameWindow, _Player);
            }
        }
        return _TestRobot;

    }
    public Bullet RandomBullet()
    {
        
        Bullet _TestBullet = new Bullet(_GameWindow,_Player);
        return _TestBullet;

    }
    public void UpdateInfo()
    {
        if (IsServer)
        {
            SplashKit.AcceptAllNewConnections();
        }



        _otherPlayerMsg = boardcast.GetNewMessages();

        UpdatePlayer();

       
        BroadcastMessage();
    }

    public void UpdatePlayer()
    {
        if (_otherPlayerMsg != null && _otherPlayerMsg.Length > 0)
        {
            name = _otherPlayerMsg.Split(',')[0];
            x = Convert.ToDouble(_otherPlayerMsg.Split(',')[1]);
            y = Convert.ToDouble(_otherPlayerMsg.Split(',')[2]);
            radius = Convert.ToDouble(_otherPlayerMsg.Split(',')[3]);
            score = Convert.ToInt32(_otherPlayerMsg.Split(',')[4]);
            //Console.WriteLine($"{name},{x},{y},{radius},{score}");
        }

        if (name.Length > 0)
        {
            if (!_otherNetworkNames.Contains(name))
            {
                _otherNetworkNames.Add(name);
                _networkPlayer.Add(new NetworkPlayer(name, x, y, radius, score, Color.Gray));
            }
        }
    }


    public void playerinfo()
    {
        foreach (NetworkPlayer n in _networkPlayer)
        {
            n.X = x;
            n.Y = y;
            n.Radius = radius;
            n.Score = score;
        }
    }

    private void BroadcastMessage()
    {
        radius = 5;
        boardcast.Broadcast($"{_Player.Name},{_Player.X},{_Player.Y},{_Player.Radius},{_Player.Score}");
    }
    
    
    private void MakeNewConnection(Network peer)
    {
        string address;
        ushort port;

        Console.Write("Enter Host Server address: ");
        address = Console.ReadLine();

        Console.Write("Enter Host Server port: ");
        port = Convert.ToUInt16(Console.ReadLine());

        peer.ConnectionSever(address, port);
    }
    
}