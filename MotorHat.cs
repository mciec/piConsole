using System;
using System.Device.I2c;
using System.Device.Pwm;
using Iot.Device.Pwm;

namespace Mciec.Drivers
{

    enum Direction
    {
        Left, Right
    };
    public class MotorHat : IDisposable
    {

        private int _i2cAddress;
        private PwmChannel _aChannelSpeed, _aChannelIn1, _aChannelIn2;
        private I2cDevice _i2cDevice;
        private Pca9685 _pca9685;

        private static int PWMA = 0;
        private static int AIN1 = 1;
        private static int AIN2 = 2;
        private static int PWMB = 3;
        private static int BIN1 = 4;
        private static int BIN2 = 5;
        private static double PwmOff = (double)(4095.0/4096.0);
        private double _speed = 0;

        public MotorHat(int i2cAddress)
        {
            _i2cAddress = i2cAddress;
        }

        public void Init()
        {
            int busId = 1;
            I2cConnectionSettings settings = new I2cConnectionSettings(busId, _i2cAddress);
            _i2cDevice = I2cDevice.Create(settings);
            _pca9685 = new Pca9685(_i2cDevice, pwmFrequency: 50);

            _aChannelSpeed = _pca9685.CreatePwmChannel(PWMA);   //motor A speed
            _aChannelIn1 = _pca9685.CreatePwmChannel(AIN1);   //motor A IN1
            _aChannelIn2 = _pca9685.CreatePwmChannel(AIN2);   //motor A IN2

            _aChannelSpeed.DutyCycle = 0.0;
            // _aChannelSpeed.Start();
            // _aChannelIn1.Start();
            // _aChannelIn2.Start();
        }

        public double Speed
        {
            get => _speed;
            set
            {
                double in1 = value >= 0 ? 0 : PwmOff;
                if (Math.Sign(_speed) != Math.Sign(value))
                {
                    _aChannelIn1.DutyCycle = in1;
                    _aChannelIn2.DutyCycle = PwmOff - in1;
                }
                _speed = value;
                _aChannelSpeed.DutyCycle = Math.Abs(_speed);
            }
        }

        public void Dispose()
        {
            if (_speed != 0 && _aChannelSpeed != null)
                _aChannelSpeed.DutyCycle = 0;
            _aChannelSpeed?.Dispose();
            _aChannelIn1?.Dispose();
            _aChannelIn2?.Dispose();
            _pca9685?.Dispose();
            _i2cDevice?.Dispose();
        }
    }
}

