using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Iot.Device.Multiplexing;

namespace led_bar_graph
{
    public class AnimateLeds
    {
        private CancellationToken _cancellation;
        private IOutputSegment _segment;
        private int[] _leds;
        private int[] _ledsReverse;
        public int LitTimeDefault = 200;
        public int DimTimeDefault = 50;
        public int LitTime = 200;
        public int DimTime = 50;

        public AnimateLeds(IOutputSegment outputSegment, CancellationToken token)
        {        
            _segment = outputSegment;
            _cancellation = token;
            _leds = Enumerable.Range(0,_segment.Length).ToArray();
            _ledsReverse = _leds.Reverse().ToArray();
        }

        private void CycleLeds(IEnumerable<int> leds, int litTime, int dimTime)
        {

            var ledSegment = leds.ToArray();
            // light time
            int index = 0;
            while (index < ledSegment.Length && !_cancellation.IsCancellationRequested)
            {
                _segment.Write(index, 1);
                index++;
            }

            _segment.Display(_cancellation, litTime);

            // dim time
            index = 0;
            while (index < ledSegment.Length && !_cancellation.IsCancellationRequested)
            {
                _segment.Write(index, 0);
                index++;
            }

            _segment.Display(_cancellation, dimTime);
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
                CycleLeds(pin,LitTime,DimTime);
            }
        }

        public void FrontToBack(bool skipLast = false)
        {
            Console.WriteLine(nameof(FrontToBack));
            var iterations = skipLast ? _segment.Length : _segment.Length - 2;
            Sequence(_leds.AsSpan(0,iterations).ToArray());
        }
        public void BacktoFront()
        {
            Console.WriteLine(nameof(BacktoFront));
            Sequence(_ledsReverse);
        }

        public void MidToEnd()
        {
            Console.WriteLine(nameof(MidToEnd));
            var half = _segment.Length / 2;

            if (_segment.Length % 2 == 1)
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
            var half = _segment.Length / 2;

            for (var i = 0; i < half ; i++)
            {
                var ledA = i;
                var ledB = _segment.Length - 1 - i;

                CycleLeds(ledA, ledB);
            }

            if (_segment.Length % 2 == 1)
            {
                CycleLeds(half);
            }
        }

        public void LightAll()
        {
            Console.WriteLine(nameof(LightAll));
            int index = 0;
            while (index < _leds.Length && !_cancellation.IsCancellationRequested)
            {
                _segment.Write(index, 1);
                index++;
            }
            _segment.Display(_cancellation,LitTime);
        }

        public void DimAllAtRandom()
        {
            Console.WriteLine(nameof(DimAllAtRandom));

            foreach (var pin in SelectRandomPins(_leds.Length))
            {
                _segment.Write(pin, 0);
                _segment.Display(_cancellation, DimTime);
                
                if (_cancellation.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        public IEnumerable<int> SelectRandomPins(int count, bool preferPairs = false)
        {
            var random = new Random();
            var pinList = _leds.ToList();
            var lastWinner = -1;

            while (pinList.Count > _leds.Length - count)
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
            
                var pin = random.Next(_segment.Length);

                if (pinList.Remove(pin))
                {
                    lastWinner = pin;
                    yield return pin;
                }
            }
        }
    }
}