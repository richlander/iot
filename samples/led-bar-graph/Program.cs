using System;
using System.Device.Gpio;
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

            using var controller = new GpioController();
            foreach(var pin in pins)
            {
                controller.OpenPin(pin, PinMode.Output);
            }
            Console.CancelKeyPress += (s, e) => 
            { 
                e.Cancel = true;
                foreach (var pin in pins)
                {
                    controller.ClosePin(pin);
                }
            };
                      
            var litTime = 200;
            var dimTime = 50;
            AnimateLed.Controller = controller;

            Console.WriteLine($"Animate! {pins.Length} pins in use.");

            while (true)
            {
                Console.WriteLine($"Lit: {litTime}ms; Dim: {dimTime}");
                AnimateLed.FrontToBack(litTime,dimTime,pins,true);
                AnimateLed.BacktoFront(litTime, dimTime, pins);
                AnimateLed.Sequence(litTime, dimTime, Enumerable.Range(1,2));
                AnimateLed.MidToEnd(litTime,dimTime,pins);
                AnimateLed.EndToMid(litTime, dimTime, pins);
                AnimateLed.MidToEnd(litTime, dimTime, pins);
                AnimateLed.LightAll(litTime,dimTime,pins);
                AnimateLed.DimAllAtRandom(litTime, dimTime, pins);

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
