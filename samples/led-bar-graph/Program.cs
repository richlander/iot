using System;
using System.Linq;
using System.Threading;
using Iot.Device.Multiplexing;

namespace led_bar_graph
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // For LED light bar graph
            // https://www.adafruit.com/product/1815
            // https://www.sparkfun.com/products/9938

            // It is easier to use the GPIO pins that are marked.
            // For example, GPIO pins 4 and 5 are marked, while GPIO pins 2 and 3 are not.
            // See BCM pins @ https://pinout.xyz/pinout/wiringpi for complete set.
            
            
            // This group of pins is intended for one LED bar graph
            // var pins = new int[] {4,5,6,12,13,16,17,18,19,20};

            // For two LED bar graphs, you have to use unmarked pins (to get to 20).
            // It is easier to start from pin 2 and use them in sequence.
            
            var cancellationSource = new CancellationTokenSource(new TimeSpan(0, 5, 0));
            var token = cancellationSource.Token;
            // IOutputSegment = new GpioSegment(pins);
            IOutputSegment segment = new ShiftRegister(ShiftRegisterPinMapping.Minimal, 8);
            var leds = new AnimateLeds(segment, token);
            Console.CancelKeyPress += (s, e) => 
            { 
                e.Cancel = true;
                cancellationSource.Cancel();
            };
                      
            Console.WriteLine($"Animate! {segment.Length} pins are initialized.");

            while (!cancellationSource.IsCancellationRequested)
            {
                Console.WriteLine($"Lit: {leds.LitTime}ms; Dim: {leds.DimTime}");
                leds.FrontToBack(true);
                leds.BacktoFront();
                leds.MidToEnd();
                leds.EndToMid();
                leds.MidToEnd();
                leds.LightAll();
                leds.DimAllAtRandom();

                if (leds.LitTime < 20)
                {
                    leds.ResetTime();
                }
                else
                {
                    leds.LitTime = (int)(leds.LitTime * 0.7);
                    leds.DimTime = (int)(leds.DimTime * 0.7);
                }
            }

            // Dim all leds
            for (int i = 0; i < segment.Length; i++)
            {
                segment.Write(i,0, default(CancellationToken), 0);
            }
        }
    }
}
