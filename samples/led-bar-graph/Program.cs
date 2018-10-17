using System;
using System.Devices.Gpio;
using System.Threading;
using System.Linq;

namespace led_bar_graph
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pins = new int[] {4,5,6,12,13,16,17,18,19,20};

            var controller = new GpioController(PinNumberingScheme.Gpio);

            controller.OpenPins(PinMode.Output, pins);

            var pinArray = controller.OpenPins.ToArray();
            var litTime = 200;
            var dimTime = 50;

            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
            {
                foreach (var pin in pins)
                {
                    controller.ClosePin(pin);
                }
            };

            Console.WriteLine($"Animate! {pins.Length} pins in use.");

            while (true)
            {
                Console.WriteLine($"Lit: {litTime}ms; Dim: {dimTime}");
                AnimateLed.FrontToBack(litTime,dimTime,pinArray,true);
                AnimateLed.BacktoFront(litTime, dimTime, pinArray);
                AnimateLed.Sequence(litTime, dimTime, pinArray,Enumerable.Range(1,2));
                AnimateLed.MidToEnd(litTime,dimTime,pinArray);
                AnimateLed.EndToMid(litTime, dimTime, pinArray);
                AnimateLed.MidToEnd(litTime, dimTime, pinArray);
                AnimateLed.LightAll(litTime,dimTime,pinArray);
                AnimateLed.DimAllAtRandom(litTime, dimTime, pinArray);

                if (litTime < 20)
                {
                    litTime = 200;
                    dimTime = 100;
                }
                else
                {
                    litTime = (int)(litTime * 0.7);
                    dimTime = (int)(dimTime * 0.7);
                }
            }
        }
    }
}
