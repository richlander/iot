using System;
using System.Device.Gpio;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Iot.Device.Multiplexing;

namespace led_bar_graph
{
    public class AnimateLeds
    {
        private CancellationToken _cancellation;
        private IOutputSegment _pinSegment;
        private int[] _pins;
        private int[] _pinsReverse;
        public int LitTimeDefault = 200;
        public int DimTimeDefault = 50;
        public int LitTime = 200;
        public int DimTime = 50;

        public AnimateLeds(IOutputSegment pinSegment, CancellationToken token)
        {        
            _pinSegment = pinSegment;
            _cancellation = token;
            _pins = Enumerable.Range(0,_pinSegment.Length).ToArray();
            _pinsReverse = _pins.Reverse().ToArray();
        }

        private void CycleLeds(IEnumerable<int> pins, int litTime, int dimTime)
        {
            if (_cancellation.IsCancellationRequested)
            {
                return;
            }

            // light time
            foreach (var pin in pins)
            {
                _pinSegment.Write(pin, 1);
            }
            Thread.Sleep(litTime);

            // dim time
            foreach (var pin in pins)
            {
                _pinSegment.Write(pin, 0);
            }
            Thread.Sleep(dimTime);
        }

        private void CycleLeds(params int[] pins)
        {
            CycleLeds(pins,LitTime,DimTime);
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
            var iterations = skipLast ? _pinSegment.Length : _pinSegment.Length - 2;
            Sequence(_pins.AsSpan(0,iterations).ToArray());
        }
        public void BacktoFront()
        {
            Console.WriteLine(nameof(BacktoFront));
            Sequence(_pinsReverse);
        }

        public void MidToEnd()
        {
            Console.WriteLine(nameof(MidToEnd));
            var half = _pinSegment.Length / 2;

            if (_pinSegment.Length % 2 == 1)
            {
                CycleLeds(half);
            }

            for (var i = 1; i < half+1; i ++)
            {
                var pinA= half - i;
                var pinB = half - 1 + i;

                CycleLeds(pinA,pinB);
            }
        }

        public void EndToMid()
        {
            Console.WriteLine(nameof(EndToMid));
            var half = _pinSegment.Length / 2;

            for (var i = 0; i < half ; i++)
            {
                var ledA = i;
                var ledB = _pinSegment.Length - 1 - i;

                CycleLeds(ledA, ledB);
            }

            if (_pinSegment.Length % 2 == 1)
            {
                CycleLeds(half);
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
                _pinSegment.Write(pin, 1);
            }
            Thread.Sleep(LitTime);
        }

        public void DimAllAtRandom()
        {
            Console.WriteLine(nameof(DimAllAtRandom));

            foreach (var pin in SelectRandomPins(_pins.Length))
            {
                _pinSegment.Write(pin, 0);
                Thread.Sleep(DimTime);
                
                if (_cancellation.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        public IEnumerable<int> SelectRandomPins(int count, bool preferPairs = false)
        {
            var random = new Random();
            var pinList = _pins.ToList();
            var lastWinner = -1;

            while (pinList.Count > _pins.Length - count)
            {
                if (_cancellation.IsCancellationRequested)
                {
                    yield break;
                }

                if (preferPairs &&  lastWinner > -1)
                {
                    var winner = lastWinner;
                    if (pinList.Remove(lastWinner + 1))
                    {
                        lastWinner  = -1;
                        yield return winner + 1;
                    }
                    else if (pinList.Remove(lastWinner -1))
                    {
                        lastWinner = -1;
                        yield return winner -1;
                    }
                }
            
                var pin = random.Next(_pinSegment.Length);

                if (pinList.Remove(pin))
                {
                    lastWinner = pin;
                    yield return pin;
                }
            }
        }
    }
}