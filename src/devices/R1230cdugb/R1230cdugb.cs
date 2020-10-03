// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Threading;
using Iot.Device.Multiplexing;

namespace Iot.Device.Multiplexing
{
    /// <summary>
    /// Binding for KWL-R1230CDUGB 12 segment light bar display
    /// </summary>
    public class R1230cdugb
    {
        // Datasheet: https://cdn-shop.adafruit.com/datasheets/KWL-R1230XDUGB.pdf
        private readonly int[] _cc;
        private IOutputSegment _segment;
        private GpioController _controller;
        private CancellationTokenSource _cancellation;

        /// <summary>
        /// Initialize a new R1230cdugb connected through a shift register.
        /// </summary>
        public R1230cdugb(ShiftRegister shiftRegister)
        {
            _segment = shiftRegister;
            _cc = new int[] { 13, 19, 26 };
            Color = 0;
            _cancellation = new CancellationTokenSource();
            _controller = new GpioController();

            foreach (int pin in _cc)
            {
                _controller.OpenPin(pin, PinMode.Input);
            }
        }

        /// <summary>
        /// Default color for R1230cdugb.
        /// Valid values are 0 (red), 1 (green), 2 (orange)
        /// Set to 0 initally.
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// Write value (low or high) to LED.
        /// Supports setting one of three colors.
        /// </summary>
        public void Write(int led, PinValue value, int color = -1)
        {
            /*
                The 12 LEDs are modeled in terms of thirds.
                The common cathode pin selects which third will get let up.

                LED 0: R2, G13
                LED 1: R3, G12
                LED 2: R4, G11
                LED 3: R5, G10

                Common cathode pins (only 1 is required for each third):

                First : 1, 14
                Second: 6, 9
                Third : 7, 8

            */

            var cc = led % 4;
            var token = _cancellation.Token;

            _controller.SetPinMode(cc, PinMode.Output);
            _segment.Write(led, value, token, 100);
        }

    }
}
