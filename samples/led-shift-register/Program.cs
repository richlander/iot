using System;
using System.Device.Gpio;
using System.Threading;

namespace led_shift_register
{
    class Program
    {

        private static GpioController _controller;
        private static int PinRCLK = 18;
        private static int PinSDI = 22;
        private static int PinSRCLK = 27;

        static void Main(string[] args)
        {
            var cancellationSource = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cancellationSource.Cancel();
            };

            _controller = new GpioController();

            var LED = new byte[] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
            var pins = new int[] { PinRCLK,PinSDI,PinSRCLK};

            foreach (var pin in pins)
            {
                _controller.OpenPin(pin, PinMode.Output);
                _controller.Write(pin,PinValue.Low);
            }

            while(!cancellationSource.IsCancellationRequested)
            {
                // flow the leds
                foreach (var led in LED)
                {
                    SIPO(led);
                    PulseRCLK();
                    Thread.Sleep(50);
                }
            }

        }

        // Serial-In-Parallel-Out
        static void SIPO(byte b)
        {
            for (var i = 0; i < 8; i++)
            {
                _controller.Write(PinSDI,(b & (0x80 >> i)) > 0 ? PinValue.High : PinValue.Low);
                PulseSRCLK();
            }
        }

        // Pulse Register Clock
        static void PulseRCLK()
        {
            _controller.Write(PinRCLK,PinValue.Low);
            _controller.Write(PinRCLK,PinValue.High);
        }

        // Pulse Serial Clock
        static void PulseSRCLK()
        {
            _controller.Write(PinSRCLK,PinValue.Low);
            _controller.Write(PinSRCLK,PinValue.High);
        }
    }
}
