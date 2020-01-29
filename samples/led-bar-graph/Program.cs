using System;
using System.Threading;

namespace led_bar_graph
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pins = new int[] {4,5,6,12,13,16,17,18,19,20};
            var cancellationSource = new CancellationTokenSource();
            using var leds = new AnimateLeds(pins, cancellationSource.Token);
            Console.CancelKeyPress += (s, e) => 
            { 
                e.Cancel = true;
                cancellationSource.Cancel();
            };
                      
            Console.WriteLine($"Animate! {pins.Length} pins are initialized.");

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
        }
    }
}
