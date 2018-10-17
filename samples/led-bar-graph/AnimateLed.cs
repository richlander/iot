using System;
using System.Devices.Gpio;
using System.Threading;
using System.Linq;

public class AnimateLed
{
    public static void FrontToBack(int litTime, int dimTime, GpioPin[] pins, bool skipLast = false)
    {
        var iterations = pins.Length;
        if (skipLast)
        {
            iterations = iterations-2;
        }

        for (var i = 0 ; i < iterations; i++)
        {
            var pin = pins[i];
            pin.Write(PinValue.High);
            Thread.Sleep(litTime);          
            pin.Write(PinValue.Low);
            Thread.Sleep(dimTime);
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

        Console.WriteLine($"half: {half}");

        if (pins.Length % 2 == 1)
        {
            Console.WriteLine($"odd number; half {half}");
            pins[half].Write(PinValue.High);
            Thread.Sleep(litTime);
            pins[half].Write(PinValue.Low);
            Thread.Sleep(dimTime);
        }

        // 5
        for (var i = 1; i < half+1; i ++)
        {
            var ledA= half - i;
            var ledB = half - 1 + i;

            Console.WriteLine($"i: {i}; half: {half}");
            // 4
            pins[ledA].Write(PinValue.High);
            // 5
            pins[ledB].Write(PinValue.High);
            Thread.Sleep(litTime);
            pins[ledA].Write(PinValue.Low);
            pins[ledB].Write(PinValue.Low);
            Thread.Sleep(dimTime);
        }
    }

    public static void EndToMid(int litTime, int dimTime, GpioPin[] pins)
    {
        var half = pins.Length / 2;

        Console.WriteLine($"half: {half}");

        // 5
        for (var i = 0; i < half ; i++)
        {
            var ledA = i;
            var ledB = pins.Length - 1 - i;

            Console.WriteLine($"i: {i}; half: {half}");
            // 4
            pins[ledA].Write(PinValue.High);
            // 5
            pins[ledB].Write(PinValue.High);
            Thread.Sleep(litTime);
            pins[ledA].Write(PinValue.Low);
            pins[ledB].Write(PinValue.Low);
            Thread.Sleep(dimTime);
        }

        if (pins.Length % 2 == 1)
        {
            Console.WriteLine($"odd number; half {half}");
            pins[half].Write(PinValue.High);
            Thread.Sleep(litTime);
            pins[half].Write(PinValue.Low);
            Thread.Sleep(dimTime);
        }
    }

}