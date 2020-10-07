// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Threading;
using Iot.Device.Multiplexing;

namespace BarGraphDriver
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
            Console.WriteLine("Hello World!");
            CancellationTokenSource cts = new CancellationTokenSource(new TimeSpan(0, 0, 20));
            var token = cts.Token;
            ShiftRegister sr = new ShiftRegister(ShiftRegisterPinMapping.Minimal, 8);
            R1230cdugb led = new R1230cdugb(sr);
            var delay = 500;
            led.Write(0, 1, 1, token, delay);
            led.Write(0, 1, 2, token, delay);
            led.Write(1, 1, 2, token, delay);
            led.Write(2, 1, 1, token, delay);
            led.Write(3, 1, 0, token, delay);
            led.Write(1, 0, 2, token, delay);
            led.Display(token);
        }
    }
}
