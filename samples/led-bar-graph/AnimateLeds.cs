using System;
using System.Device.Gpio;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

public class AnimateLeds
{
    private GpioController _controller;
    public CancellationToken Cancellation;

    public AnimateLeds(GpioController controller)
    {
        _controller = controller;
    }

    public void Init(int[] pins)
    {
        foreach (var pin in pins)
        {
            _controller.OpenPin(pin, PinMode.Output);
        }
    }

    private void CycleLeds(int litTime, int dimTime, params int[] pins)
    {
        if (Cancellation.IsCancellationRequested)
        {
            return;
        }

        // light time
        foreach (var pin in pins)
        {
            _controller.Write(pin, PinValue.High);
        }
        Thread.Sleep(litTime);

        if (Cancellation.IsCancellationRequested)
        {
            return;
        }

        // dim time
        foreach (var pin in pins)
        {
            _controller.Write(pin, PinValue.Low);
        }
        Thread.Sleep(dimTime);
    }

    public void Sequence(int litTime, int dimTime, IEnumerable<int> pins)
    {
        Console.WriteLine(nameof(Sequence));
        foreach (var pin in pins)
        {
            CycleLeds(litTime, dimTime, pin);
        }
    }

    public void FrontToBack(int litTime, int dimTime, int[] pins, bool skipLast = false)
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
    public void BacktoFront(int litTime, int dimTime, int[] pins, bool skipLast = false)
    {
        Console.WriteLine(nameof(BacktoFront));
        var reverseArray = pins.Reverse().ToArray();
        FrontToBack(litTime,dimTime,reverseArray, skipLast);
    }

    public void MidToEnd(int litTime, int dimTime, int[] pins)
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

    public void EndToMid(int litTime, int dimTime, int[] pins)
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

    public void LightAll(int litTime, int dimTime, int[] pins)
    {
        Console.WriteLine(nameof(LightAll));
        foreach(var pin in pins)
        {
            if (Cancellation.IsCancellationRequested)
            {
                return;
            }

            _controller.Write(pin, PinValue.High);
        }
        Thread.Sleep(litTime);
    }

    public void DimAllAtRandom(int dimTime, int[] pins)
    {
        Console.WriteLine(nameof(DimAllAtRandom));
        var random = new Random();

        var ledList = Enumerable.Range(0,pins.Length).ToList();

        while (ledList.Count > 0)
        {
            if (Cancellation.IsCancellationRequested)
            {
                return;
            }

            var led = random.Next(pins.Length);

            if (ledList.Remove(led))
            {
                _controller.Write(pins[led],PinValue.Low);
                Thread.Sleep(dimTime);
            }
        }

    }

}
