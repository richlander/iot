using System;
using System.Device.Gpio;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

public class AnimateLeds : IDisposable
{
    private GpioController _controller;
    private CancellationToken _cancellation;
    private int[] _pins;

    public int LitTimeDefault = 200;
    public int DimTimeDefault = 50;
    public int LitTime = 200;
    public int DimTime = 50;

    public AnimateLeds(int[] pins, CancellationToken token)
    {
        _controller = new GpioController();
        _pins = pins;
        _cancellation = token;
        
        foreach (var pin in _pins)
        {
            _controller.OpenPin(pin, PinMode.Output);
        }    
    }

    private void CycleLeds(params int[] pins)
    {
        if (_cancellation.IsCancellationRequested)
        {
            return;
        }

        // light time
        foreach (var pin in pins)
        {
            _controller.Write(pin, PinValue.High);
        }
        Thread.Sleep(LitTime);

        // dim time
        foreach (var pin in pins)
        {
            _controller.Write(pin, PinValue.Low);
        }
        Thread.Sleep(DimTime);
    }

    public void ResetTime()
    {
        LitTime = LitTimeDefault;
        DimTime = DimTimeDefault;
    }

    public void Sequence(IEnumerable<int> pins)
    {
        Console.WriteLine(nameof(Sequence));
        foreach (var pin in pins)
        {
            CycleLeds(pin);
        }
    }

    public void FrontToBack(bool skipLast = false)
    {
        Console.WriteLine(nameof(FrontToBack));
        var iterations = skipLast ? _pins.Length : _pins.Length - 2;
        Sequence(_pins.AsSpan(0,iterations).ToArray());
    }
    public void BacktoFront()
    {
        Console.WriteLine(nameof(BacktoFront));
        Sequence(_pins.Reverse().ToArray());
    }

    public void MidToEnd()
    {
        Console.WriteLine(nameof(MidToEnd));
        var half = _pins.Length / 2;

        if (_pins.Length % 2 == 1)
        {
            CycleLeds(_pins[half]);
        }

        for (var i = 1; i < half+1; i ++)
        {
            var ledA= half - i;
            var ledB = half - 1 + i;

            CycleLeds(_pins[ledA],_pins[ledB]);
        }
    }

    public void EndToMid()
    {
        Console.WriteLine(nameof(EndToMid));
        var half = _pins.Length / 2;

        for (var i = 0; i < half ; i++)
        {
            var ledA = i;
            var ledB = _pins.Length - 1 - i;

            CycleLeds(_pins[ledA], _pins[ledB]);
        }

        if (_pins.Length % 2 == 1)
        {
            CycleLeds(_pins[half]);
        }
    }

    public void LightAll()
    {
        Console.WriteLine(nameof(LightAll));
        foreach(var pin in _pins)
        {
            if (_cancellation.IsCancellationRequested)
            {
                return;
            }

            _controller.Write(pin, PinValue.High);
        }
        Thread.Sleep(LitTime);
    }

    public void DimAllAtRandom()
    {
        Console.WriteLine(nameof(DimAllAtRandom));
        var random = new Random();

        var ledList = Enumerable.Range(0,_pins.Length).ToList();

        while (ledList.Count > 0)
        {
            if (_cancellation.IsCancellationRequested)
            {
                return;
            }

            var led = random.Next(_pins[_pins.Length-1]);

            if (ledList.Remove(led))
            {
                _controller.Write(_pins[led],PinValue.Low);
                Thread.Sleep(DimTime);
            }
        }

    }

    public void Dispose()
    {
        _controller.Dispose();
    }
}
