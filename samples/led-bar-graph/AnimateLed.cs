using System;
using System.Devices.Gpio;
using System.Threading;
using System.Linq;

public class AnimateLed
{
    public static void FrontToBack(int litTime, int dimTime, GpioPin[] pins)
    {
        foreach(var pin in pins)
        {
            pin.Write(PinValue.High);
            Thread.Sleep(litTime);
            pin.Write(PinValue.Low);
            Thread.Sleep(dimTime);
        }
    }

    public static void BacktoFront(int litTime, int dimTime, GpioPin[] pins)
    {
        var reverseArray = pins.Reverse().ToArray();
        FrontToBack(litTime,dimTime,reverseArray);
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
            Console.WriteLine($"i: {i}; half: {half}");
            // 4
            pins[half - i].Write(PinValue.High);
            // 5
            pins[half-1 + i].Write(PinValue.High);
            Thread.Sleep(litTime);
            pins[half -i].Write(PinValue.Low);
            pins[half-1 +i].Write(PinValue.Low);
            Thread.Sleep(dimTime);
        }
    }

    public static void EndToMid(int litTime, int dimTime, GpioPin[] pins)
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
        for (var i = 1; i < half + 1; i++)
        {
            Console.WriteLine($"i: {i}; half: {half}");
            // 4
            pins[half - i].Write(PinValue.High);
            // 5
            pins[half - 1 + i].Write(PinValue.High);
            Thread.Sleep(litTime);
            pins[half - i].Write(PinValue.Low);
            pins[half - 1 + i].Write(PinValue.Low);
            Thread.Sleep(dimTime);
        }
    }

}