﻿using System;
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
            var cancellationSource = new CancellationTokenSource();
            using var controller = new GpioController();
            var leds = new AnimateLeds(controller);
            leds.Cancellation = cancellationSource.Token;
            leds.Init(pins);
            Console.CancelKeyPress += (s, e) => 
            { 
                e.Cancel = true;
                cancellationSource.Cancel();
                Thread.Sleep(50);
                var leds2 = new AnimateLeds(controller);
                leds2.DimAllAtRandom(20, pins);
            };
                      
            var litTime = 200;
            var dimTime = 50;
            Console.WriteLine($"Animate! {pins.Length} pins in use.");

            while (leds.Cancellation.IsCancellationRequested)
            {
                Console.WriteLine($"Lit: {litTime}ms; Dim: {dimTime}");
                leds.FrontToBack(litTime,dimTime,pins,true);
                leds.BacktoFront(litTime, dimTime, pins);
                leds.MidToEnd(litTime,dimTime,pins);
                leds.EndToMid(litTime, dimTime, pins);
                leds.MidToEnd(litTime, dimTime, pins);
                leds.LightAll(litTime,dimTime,pins);
                leds.DimAllAtRandom(dimTime, pins);

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
