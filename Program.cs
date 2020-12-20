using System;
using System.Device.I2c;
using System.Device.Pwm;
using Iot.Device.Pwm;


namespace console01
{
    class Program
    {
        private static int PWMA = 0;
        private static int AIN1 = 1;
        private static int AIN2 = 2;
        private static int PWMB = 3;
        private static int BIN1 = 4;
        private static int BIN2 = 5;
        static void Main(string[] args)
        {
            Console.WriteLine($"Hello from {Environment.MachineName}");

            Console.WriteLine($"Hello from {Environment.MachineName}");


            int busId = 1;
            var selectedI2cAddress = 0b1000000; // A6=1 A5=0 A4 A3 A2 A1 A0
            I2cConnectionSettings settings = new I2cConnectionSettings(busId, selectedI2cAddress);
            using (var i2cDevice = I2cDevice.Create(settings))
            {
                using (var pca9685 = new Pca9685(i2cDevice, pwmFrequency: 50))
                {
                    PwmChannel aChannelSpeed = pca9685.CreatePwmChannel(PWMA);   //motor A speed
                    PwmChannel aChannelIn1 = pca9685.CreatePwmChannel(AIN1);   //motor A IN1
                    PwmChannel aChannelIn2 = pca9685.CreatePwmChannel(AIN2);   //motor A IN2

                    PwmChannel bChannel = pca9685.CreatePwmChannel(PWMB); //motor B speed
                    PwmChannel bChannelIn1 = pca9685.CreatePwmChannel(BIN1);   //motor B IN1
                    PwmChannel bChannelIn2 = pca9685.CreatePwmChannel(BIN2);   //motor B IN2

                    aChannelSpeed.Start();
                    aChannelIn1.Start();
                    aChannelIn2.Start();
                    aChannelSpeed.DutyCycle = 0.3;
                    aChannelIn1.DutyCycle = 0;
                    aChannelIn2.DutyCycle = 1;
                    aChannelSpeed.Start();
                    aChannelIn1.Start();
                    aChannelIn2.Start();

                    pca9685.SetDutyCycle(PWMA, 1);
                    pca9685.SetDutyCycle(AIN1, 0);
                    pca9685.SetDutyCycle(AIN2, 1);

                    pca9685.SetDutyCycle(PWMB, 1);
                    pca9685.SetDutyCycle(BIN1, 0);
                    pca9685.SetDutyCycle(BIN2, 1);



                }
            }
        }
    }
}