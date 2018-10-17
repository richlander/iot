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

            foreach (var pin in pins)
            {
                controller.OpenPin(pin,PinMode.Output);
            }

            var pinArray = controller.OpenPins.ToArray();
            var litTime = 200;
            var dimTime = 50;

            while (true)
            {
                AnimateLed.FrontToBack(litTime,dimTime,pinArray,true);
                AnimateLed.BacktoFront(litTime, dimTime, pinArray);
                AnimateLed.MidToEnd(litTime,dimTime,pinArray);
                AnimateLed.EndToMid(litTime, dimTime, pinArray);
                
                if (litTime < 5)
                {
                    litTime = 200;
                    dimTime = 100;
                }
                else
                {
                    litTime /= 2;
                    dimTime /= 2;
                }
            }
        }
    }
}
