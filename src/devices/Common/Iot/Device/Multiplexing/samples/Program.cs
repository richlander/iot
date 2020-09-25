// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using Iot.Device.Multiplexing;

namespace GpioOutputSegmentDriver
{
    /// <summary>
    /// Test application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entrypoint
        /// </summary>
        public static void Main(string[] args)
        {
            int[] pins = new int[] { 16, 20, 21 };

            GpioOutputSegment segment = new GpioOutputSegment(pins);
            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                tokenSource.Cancel();
            };

            var pinValue = 1;

            while (!ct.IsCancellationRequested)
            {
                Console.WriteLine($"{nameof(pinValue)}: {pinValue}");
                int index = 0;
                while (index < pins.Length && !ct.IsCancellationRequested)
                {
                    segment.Write(index, pinValue, ct, 500);
                    index++;
                }

                segment.Display(ct, 1000);
                pinValue = (pinValue + 1) % 2;
            }

            segment.Clear();
        }
    }
}
