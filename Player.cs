using System;
using SplashKitSDK;

public class Player
{
    private Bitmap _PlayerBitmap;
    private Bitmap _LifeBitmap;
    public string Name { get; set; }
    public int Score;
    private int _NameWidth;
    public Circle c;
    public int Lives = 150;
    public bool fire = false;
    public double Radius { get; set; } = 10;
    
    
    //initializing setter and getter

    public double X { get; private set; }

    public double Y { get; private set; }

    public int Width
    {
        get => _PlayerBitmap.Width;
    }

    public int Height
    {
        get => _PlayerBitmap.Height;
    }

    public bool Quit { get; set; }


//initializing constructor with window object
    public Player(Window gameWindow,string name)
    {
        //decalring Bitmap
        _PlayerBitmap = new Bitmap("Player", "Player.png");
        _LifeBitmap =new Bitmap("life","Like.png");
        X = (gameWindow.Width - Width) / 2;
        Y = (gameWindow.Height - Height) / 2;
        Name = name;
        _NameWidth = SplashKit.TextWidth(name, "Arial", 10);
        
        Quit = false;
    }


    //player bitmap drawing method
    public void Draw()
    {   c = SplashKit.CircleAt(X, Y, Radius);
        SplashKit.DrawBitmap(_PlayerBitmap, X, Y);
        SplashKit.DrawBitmap(_LifeBitmap,680,520);
        SplashKit.DrawText($"{Lives}",Color.Black,"BoldFont",2,660,540);
        
        
    }

//method to handle the input from the user
    public void HandleInput()

    {
        const int SPEED = 5;
        SplashKit.ProcessEvents();

        if (SplashKit.KeyDown(KeyCode.UpKey))
        {
            Y -= SPEED;
        }

        if (SplashKit.KeyDown(KeyCode.DownKey))
        {
            Y += SPEED;
        }

        if (SplashKit.KeyDown(KeyCode.LeftKey))
        {
            X -= SPEED;
        }

        if (SplashKit.KeyDown(KeyCode.RightKey))
        {
            X += SPEED;
        }

        if (SplashKit.KeyDown(KeyCode.EscapeKey))
        {
            Quit = true;
        }

        if (SplashKit.MouseDown(MouseButton.LeftButton))
        {
            fire = true;
        }
    }

// method to limit the player on the screen, without overflow   
    public void StayOnWindow(Window limit)
    {
        const int GAP = 10;

        if (X < GAP)
        {
            X = GAP;
        }

        if (X + Width > limit.Width - GAP)
        {
            X = (limit.Width - GAP) - Width;
        }

        if ((X + Width) > limit.Width - GAP)
        {
            X = (limit.Width - GAP) - Width;
        }

        if (Y < GAP)
        {
            Y = GAP;
        }

        if ((Y + Height) > limit.Height - GAP)
        {
            Y = (limit.Height - GAP) - Height;
        }
    }

    public bool CollidedWith(Robot robo)
    {
        return _PlayerBitmap.CircleCollision(X, Y, robo.CollisionCircle);
    }
    
    public bool CollidedWith(NetworkPlayer op)
    {
        return SplashKit.CirclesIntersect(c, op.CollisionCircle);
    }
    
    
    
}