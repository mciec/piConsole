using System;
using Mciec.Drivers;

namespace console01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Hello from {Environment.MachineName}");

            using (var motorHat = new MotorHat(0b1000000))  // A6=1 A5=0 A4 A3 A2 A1 A0)))
            {
                motorHat.Init();
                string key;
                motorHat.Speed = -0.2;
                motorHat.Speed = -0.1;
                motorHat.Speed = 0.1;
                motorHat.Speed = -0.1;
                motorHat.Speed = +0.1;



                // while ((key = Console.ReadLine()) != "q")
                // {
                //     switch (key)
                //     {
                //         case "a":
                //             motorHat.Speed = motorHat.Speed + 0.1;
                //             break;
                //         case "z":
                //             motorHat.Speed = motorHat.Speed - 0.1;
                //             break;
                //     }
                //     Console.WriteLine($"Speed: {motorHat.Speed}");
                // }

            }


        }
    }
}