using System;
using Mciec.Drivers;
using Iot.Device.Hcsr04;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


namespace console01
{
    class Program
    {
        private const double SwitchDistance = 10.0;
        private static double Distance;
        static EventWaitHandle objectClose = new EventWaitHandle(false, EventResetMode.AutoReset);
        static EventWaitHandle objectFar = new EventWaitHandle(false, EventResetMode.AutoReset);

        private static async void DoPing(Object stateObject)
        {
            double prevDistance = 200.0;
            CancellationToken ct = (CancellationToken)stateObject;
            Hcsr04 sonar = new Hcsr04(18, 24);
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    Distance = sonar.Distance.Centimeters;
                }
                catch { }
                //Console.WriteLine($"Distance: {Distance}");
                if (prevDistance <= SwitchDistance && Distance > SwitchDistance)
                {
                    objectClose.Reset();
                    objectFar.Set();
                    Console.WriteLine("-------------OPEN---------------");
                }
                else
                if (prevDistance > SwitchDistance && Distance <= SwitchDistance)
                {
                    objectFar.Reset();
                    objectClose.Set();
                    Console.WriteLine("-------------CLOSE--------------");
                }
                prevDistance = Distance;
                await Task.Delay(100);
            }
        }

        private static async void ControlServo(Object stateObject)
        {
            CancellationToken ct = (CancellationToken)stateObject;

            using (var servoHat = new ServoHat(0b1000000, 15))
            {
                servoHat.Init();
                servoHat.Calibrate(180, 420, 2650);
                servoHat.WriteAngle(90);
                while (!ct.IsCancellationRequested)
                {
                    while (Distance < 10)
                    {
                        servoHat.WriteAngle(0);
                        await Task.Delay(5000);
                    }
                    servoHat.WriteAngle(90);
                }
            }
        }

        private static async void ControlServoBySignal(Object stateObject)
        {
            CancellationToken ct = (CancellationToken)stateObject;
            Stopwatch sw = new Stopwatch();
            using (var servoHat = new ServoHat(0b1000000, 15))
            {
                servoHat.Init();
                servoHat.Calibrate(180, 420, 2650);
                servoHat.WriteAngle(90);
                while (!ct.IsCancellationRequested)
                {
                    Console.WriteLine("Open. Waiting for CLOSE signal...");
                    objectClose.WaitOne();
                    servoHat.WriteAngle(0);
                    do
                    {
                        Console.WriteLine("Close. CLOSE signal received. Waiting for OPEN signal...");
                        objectFar.WaitOne();                       
                        Console.WriteLine("Close. OPEN signal received. Waiting 5sec...");
                    } while (objectClose.WaitOne(5000));                   
                    servoHat.WriteAngle(90);
                }
            }
        }

        static void Main(string[] args)
        {

            Console.WriteLine($"Hello from {Environment.MachineName}");
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            //Task<void> task= 
            Task.Factory.StartNew(stateObject => DoPing(stateObject), cancellationToken);
            Task.Factory.StartNew(stateObject => ControlServoBySignal(stateObject), cancellationToken);
            Console.ReadLine();
            cancellationTokenSource.Cancel();
            Task.Delay(5000).GetAwaiter().GetResult();
            // using (var motorHat = new MotorHat(0b1000000))  // A6=1 A5=0 A4 A3 A2 A1 A0)))
            // {
            //     motorHat.Init();
            //     string key;
            //     motorHat.Speed = -0.2;
            //     motorHat.Speed = -0.1;
            //     motorHat.Speed = 0.1;
            //     motorHat.Speed = -0.1;
            //     motorHat.Speed = +0.1;
            // }

            // using (var servoHat = new ServoHat(0b1000000, 15))
            // {
            //     servoHat.Init();
            //     //servo.Calibrate();
            //     servoHat.Calibrate(180, 420, 2650);
            //     servoHat.WriteAngle(0);
            //     int pw = 500;
            //     char key;
            //     while ((key = Console.ReadKey().KeyChar) != 'q')
            //     {
            //         switch (key)
            //         {
            //             case 'a':
            //                 pw += 10; break;
            //             case 'z':
            //                 pw -= 10; break;
            //             case 's':
            //                 pw += 50; break;
            //             case 'x':
            //                 pw -= 50; break;
            //         }
            //         servoHat.WritePulseWidth(pw);
            //         Console.WriteLine($"pulseWidth: {pw}");
            //     }
            //}
        }

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