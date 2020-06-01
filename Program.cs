using System;
using SplashKitSDK;


public class Program
{   
    public static void Main()
    {   
        //creating window object and passing into constructor of class Player 
        Window gameWindow = new Window("Player", 800, 600);
        //Constructor for class RoboDodge
        RobotDodge robo = new RobotDodge(gameWindow);

        while (!gameWindow.CloseRequested && !robo.Quit)
        {   
            SplashKit.ProcessEvents();
            robo.HandleInput();
            robo.Update();
            robo.Draw();
            
        }

        gameWindow.Close();
    }
}