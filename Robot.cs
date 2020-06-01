using System;
using SplashKitSDK;

public abstract class Robot
{
    //velocity property in vector
    private Vector2D Velocity { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public Color MainColor { get; set; }

    public int Width
    {
        get { return 50; }
    }

    public int Height
    {
        get { return 50; }
    }

    public Circle CollisionCircle
    {
        get { return SplashKit.CircleAt(X, Y, 20); }
    }


    public Robot(Window gameWindow, Player player)
    {
        if (SplashKit.Rnd() < 0.5)
        {
            //We picked...Top/Bottom

            //Start by picking the random position left to right (X)
            X = SplashKit.Rnd(gameWindow.Width);

            //Now work out if we are top or bottom?
            if (SplashKit.Rnd() < 0.5)
            {
                Y = -Height; //Top...so above top
            }
            else
            {
                Y = gameWindow.Height; //Bottom so below bottom
            }
        }
        else
        {
            //We picked..Left // Right
            Y = SplashKit.Rnd(gameWindow.Height);

            if (SplashKit.Rnd() < 0.5)
            {
                X = -Width;
            }
            else
            {
                X = gameWindow.Width;
            }
        }


        //adding velocity to robot
        const int SPEED = 4;

        //Get a point from Robot
        Point2D fromPt = new Point2D()
        {
            X = X,
            Y = Y
        };

        //Get a point from Player
        Point2D toPt = new Point2D()
        {
            X = player.X,
            Y = player.Y
        };

        //Calculate the direction to head
        Vector2D dir;
        dir = SplashKit.UnitVector(SplashKit.VectorPointToPoint(fromPt, toPt));

        //Set the speed and assign the velocity
        Velocity = SplashKit.VectorMultiply(dir, SPEED);
        MainColor = Color.RandomRGB(200);
    }

    public void Update()
    {
        X = X + Velocity.X;
        Y = Y + Velocity.Y;
    }


    abstract public void Draw();

    //To check whether a robot is off screen
    public bool IsOffscreen(Window screen)
    {
        return (X < -Width || X > screen.Width || Y < -Height || Y > screen.Height);
    }
}

class Boxy : Robot
{
    public Boxy(Window gameWindow, Player player) : base(gameWindow, player)
    {
    }

    override public void Draw()
    {
        double leftX, rightX, eyeY, mouthY;
        leftX = X + 12;
        rightX = X + 27;
        eyeY = Y + 10;
        mouthY = Y + 30;
        SplashKit.FillRectangle(Color.Gray, X, Y, 50, 50);
        SplashKit.FillRectangle(MainColor, leftX, eyeY, 10, 10);
        SplashKit.FillRectangle(MainColor, rightX, eyeY, 10, 10);
        SplashKit.FillRectangle(MainColor, leftX, mouthY, 25, 10);
        SplashKit.FillRectangle(MainColor, leftX + 2, mouthY + 2, 21, 6);
    }
}

class Roundy : Robot
{
    public Roundy(Window gameWindow, Player player) : base(gameWindow, player)
    {
    }

    public override void Draw()
    {
        double leftX, midX, rightX;
        double midY, eyeY, mouthY;
        leftX = X + 17;
        midX = X + 25;
        rightX = X + 33;
        midY = Y + 25;
        eyeY = Y + 20;
        mouthY = Y + 35;
        SplashKit.FillCircle(Color.White, midX, midY, 25);
        SplashKit.DrawCircle(Color.Gray, midX, midY, 25);
        SplashKit.FillCircle(MainColor, leftX, eyeY, 5);
        SplashKit.FillCircle(MainColor, rightX, eyeY, 5);
        SplashKit.FillEllipse(Color.Gray, X, eyeY, 50, 30);
        SplashKit.DrawLine(Color.Black, X, mouthY, X + 50, Y + 35);
    }
}

class Octopus : Robot
{
    public Octopus(Window gameWindow, Player player) : base(gameWindow, player)
    {
    }

    public override void Draw()
    {
        Bitmap RoboBitmap;
        RoboBitmap = new Bitmap("robo", "robo.png");
        SplashKit.DrawBitmap(RoboBitmap, X, Y);
    }
}