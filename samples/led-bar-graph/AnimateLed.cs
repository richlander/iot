using System;
using System.Device.Gpio;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

public class AnimateLed
{
    public static GpioController Controller;
    public static CancellationToken Cancellation;
    private static void CycleLeds(int litTime, int dimTime, params int[] leds)
    {
        if (Cancellation.IsCancellationRequested)
        {
            return;
        }

        // light time
        foreach (var led in leds)
        {
            Controller.Write(led, PinValue.High);
        }
        Thread.Sleep(litTime);

        if (Cancellation.IsCancellationRequested)
        {
            return;
        }

        // dim time
        foreach (var led in leds)
        {
            Controller.Write(led, PinValue.Low);
        }
        Thread.Sleep(dimTime);
    }

    public static void Sequence(int litTime, int dimTime, IEnumerable<int> leds)
    {
        Console.WriteLine(nameof(Sequence));
        foreach (var led in leds)
        {
            CycleLeds(litTime, dimTime, led);
        }
    }

    public static void FrontToBack(int litTime, int dimTime, int[] pins, bool skipLast = false)
    {
        Console.WriteLine(nameof(FrontToBack));
        var iterations = pins.Length;
        if (skipLast)
        {
            iterations = iterations - 2;
        }

        for (var i = 0; i < iterations; i++)
        {
            CycleLeds(litTime, dimTime, pins[i]);
        }
    }
    public static void BacktoFront(int litTime, int dimTime, int[] pins, bool skipLast = false)
    {
        Console.WriteLine(nameof(BacktoFront));
        var reverseArray = pins.Reverse().ToArray();
        FrontToBack(litTime,dimTime,reverseArray, skipLast);
    }

    public static void MidToEnd(int litTime, int dimTime, int[] pins)
    {
        Console.WriteLine(nameof(MidToEnd));
        var half = pins.Length / 2;

        if (pins.Length % 2 == 1)
        {
            CycleLeds(litTime, dimTime, pins[half]);
        }

        for (var i = 1; i < half+1; i ++)
        {
            var ledA= half - i;
            var ledB = half - 1 + i;

            CycleLeds(litTime,dimTime,pins[ledA],pins[ledB]);
        }
    }

    public static void EndToMid(int litTime, int dimTime, int[] pins)
    {
        Console.WriteLine(nameof(EndToMid));
        var half = pins.Length / 2;

        for (var i = 0; i < half ; i++)
        {
            var ledA = i;
            var ledB = pins.Length - 1 - i;

            CycleLeds(litTime, dimTime, pins[ledA], pins[ledB]);
        }

        if (pins.Length % 2 == 1)
        {
            CycleLeds(litTime, dimTime, pins[half]);
        }
    }

    public static void LightAll(int litTime, int dimTime, int[] pins)
    {
        Console.WriteLine(nameof(LightAll));
        foreach(var pin in pins)
        {
            Controller.Write(pin, PinValue.High);
        }
        Thread.Sleep(litTime);
    }

    public static void DimAllAtRandom(int litTime, int dimTime, int[] pins)
    {
        Console.WriteLine(nameof(DimAllAtRandom));
        var random = new Random();

        var ledList = Enumerable.Range(0,pins.Length).ToList();

        while (ledList.Count > 0)
        {
            var led = random.Next(pins.Length);

            if (ledList.Remove(led))
            {
                Controller.Write(pins[led],PinValue.Low);
                Thread.Sleep(dimTime);
            }
        }

    }

}
