using System;
using System.Device.I2c;
using System.Device.Pwm;
using Iot.Device.Pwm;
using Iot.Device.ServoMotor;

namespace Mciec.Drivers
{

    public class ServoHat : IDisposable
    {
        private int _i2cAddress;
        private readonly int _channelNumber;
        private PwmChannel _servoChannel;
        private I2cDevice _i2cDevice;
        private Pca9685 _pca9685;

        private ServoMotor _servoMotor;

        public ServoHat(int i2cAddress, int channelNumber)
        {
            _i2cAddress = i2cAddress;
            if (channelNumber < 0 || channelNumber > 15)
                throw new ArgumentOutOfRangeException(nameof(channelNumber), channelNumber, $"Value must be between 0 and 15.");
            _channelNumber = channelNumber;
        }

        public void Calibrate(double maximumAngle, double minimumPulseWidthMicroseconds, double maximumPulseWidthMicroseconds) =>
            _servoMotor.Calibrate(maximumAngle, minimumPulseWidthMicroseconds, maximumPulseWidthMicroseconds);


        public void Start() => _servoMotor.Start();

        /// <summary>
        /// Stops the servo motor.
        /// </summary>
        public void Stop() => _servoMotor.Stop();

        /// <summary>
        /// Writes an angle to the servo motor.
        /// </summary>
        /// <param name="angle">The angle to write to the servo motor.</param>
        public void WriteAngle(double angle) => _servoMotor.WriteAngle(angle);

        /// <summary>
        /// Writes a pulse width to the servo motor.
        /// </summary>
        /// <param name="microseconds">The pulse width, in microseconds, to write to the servo motor.</param>
        public void WritePulseWidth(int microseconds) => _servoMotor.WritePulseWidth(microseconds);
        public void Init()
        {
            int busId = 1;
            I2cConnectionSettings settings = new I2cConnectionSettings(busId, _i2cAddress);
            _i2cDevice = I2cDevice.Create(settings);
            _pca9685 = new Pca9685(_i2cDevice, pwmFrequency: 50);

            _servoChannel = _pca9685.CreatePwmChannel(_channelNumber);
            _servoMotor = new ServoMotor(_servoChannel, 180, 500, 2500);
        }

        public void Dispose()
        {
            _servoMotor?.Dispose();
            _servoChannel?.Dispose();
            _pca9685?.Dispose();
            _i2cDevice?.Dispose();
        }
    }
}

