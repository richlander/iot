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
            ShiftRegister sr = new ShiftRegister(ShiftRegisterPinMapping.Minimal, 8);
            R1230cdugb led = new R1230cdugb(sr);
            led.Write(0, 1);
        }
    }
}
