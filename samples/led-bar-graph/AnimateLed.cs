using System;
using System.Devices.Gpio;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

public class AnimateLed
{
    private static void CycleLeds(int litTime, int dimTime, GpioPin[] pins, params int[] leds)
    {
        // light time
        foreach (var led in leds)
        {
            pins[led].Write(PinValue.High);
        }
        Thread.Sleep(litTime);

        // dim time
        foreach (var led in leds)
        {
            pins[led].Write(PinValue.Low);
        }
        Thread.Sleep(dimTime);
    }

    public static void Sequence(int litTime, int dimTime, GpioPin[] pins, IEnumerable<int> leds)
    {
        foreach (var led in leds)
        {
            CycleLeds(litTime, dimTime, pins, led);
        }
    }

    public static void FrontToBack(int litTime, int dimTime, GpioPin[] pins, bool skipLast = false)
    {
        var iterations = pins.Length;
        if (skipLast)
        {
            iterations = iterations - 2;
        }

        for (var i = 0; i < iterations; i++)
        {
            CycleLeds(litTime, dimTime, pins, i);
        }
    }
    public static void BacktoFront(int litTime, int dimTime, GpioPin[] pins, bool skipLast = false)
    {
        var reverseArray = pins.Reverse().ToArray();
        FrontToBack(litTime,dimTime,reverseArray, skipLast);
    }

    public static void MidToEnd(int litTime, int dimTime, GpioPin[] pins)
    {
        var half = pins.Length / 2;

        if (pins.Length % 2 == 1)
        {
            CycleLeds(litTime, dimTime, pins, half);
        }

        for (var i = 1; i < half+1; i ++)
        {
            var ledA= half - i;
            var ledB = half - 1 + i;

            CycleLeds(litTime,dimTime,pins,ledA,ledB);
        }
    }

    public static void EndToMid(int litTime, int dimTime, GpioPin[] pins)
    {
        var half = pins.Length / 2;

        for (var i = 0; i < half ; i++)
        {
            var ledA = i;
            var ledB = pins.Length - 1 - i;

            CycleLeds(litTime, dimTime, pins, ledA, ledB);
        }

        if (pins.Length % 2 == 1)
        {
            CycleLeds(litTime, dimTime, pins, half);
        }
    }

    public static void LightAll(int litTime, int dimTime, GpioPin[] pins)
    {
        foreach(var pin in pins)
        {
            pin.Write(PinValue.High);
        }
        Thread.Sleep(litTime);
    }

    public static void DimAllAtRandom(int litTime, int dimTime, GpioPin[] pins)
    {
        var random = new Random();

        var ledList = Enumerable.Range(0,10).ToList();

        while (ledList.Count > 0)
        {
            var led = random.Next(pins.Length);

            if (ledList.Remove(led))
            {
                pins[led].Write(PinValue.Low);
                Thread.Sleep(dimTime);
            }
        }

    }

}