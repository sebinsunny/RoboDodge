using System;
using SplashKitSDK;

public class Bullet
{
    //bullet bitmap
    private Bitmap _BulletBitmap;
    public double X { get; set; }
    public double Y { get; set; }

    private Vector2D Velocity { get; set; }

    public int Width
    {
        get { return _BulletBitmap.Width; }
    }
    //read only property
    public int Height
    {
        get { return _BulletBitmap.Height; }
    }


    public Bullet(Window gameWindow, Player _Player)
    {
        _BulletBitmap = new Bitmap("Bullet", "Fire.png");

        //setting the position of the bullect
        //creating the bullet if the mouse is clicked
        //When the mouse is clicked the bullet is originated from player and move towards the position where the mouse is clicked
        
        const int SPEED = 8;
        //bullet is projected from the center of the player

        X = _Player.X + _Player.Width/2;
        Y = _Player.Y + _Player.Height/2;
        //Get a point from Bullet
        Point2D fromPt = new Point2D()
        {
            X = X,
            Y = Y
        };

        //getting the location of the mouse
        Point2D mousePt = SplashKit.MousePosition();
        
        //Calculating the direction to move bullet towards the position of the mouse clicked
        Vector2D dir = SplashKit.UnitVector(SplashKit.VectorPointToPoint(fromPt, mousePt));

        //initializing the velocity
        Velocity = SplashKit.VectorMultiply(dir, SPEED);
    }

    public void Draw()
    {
        SplashKit.ProcessEvents();
        SplashKit.DrawBitmap(_BulletBitmap, X, Y);
    }
    public void Update()
    {
        X = X + Velocity.X;
        Y = Y + Velocity.Y;
    }
    public bool IsOffscreen(Window screen)
    {
        return (X < -Width || X > screen.Width || Y < -Height || Y > screen.Height);
    }
    public bool BulletCollidedWith(Robot robot)
    {
        return _BulletBitmap.CircleCollision(X, Y, robot.CollisionCircle);
    }

}