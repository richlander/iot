// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Threading;

namespace Iot.Device.Multiplexing
{
    /// <summary>
    /// Binding for KWL-R1230CDUGB 12 segment light bar display
    /// </summary>
    public class R1230cdugb : IOutputSegment
    {
        // Unit: https://www.adafruit.com/product/1719
        // Unit: https://www.adafruit.com/product/1721 (w/I2C backpack)
        // Datasheet: https://cdn-shop.adafruit.com/datasheets/KWL-R1230XDUGB.pdf
        private readonly int[] _cc;
        private IOutputSegment _sr;
        private GpioController _controller;
        private R1230cdugbLed[] _leds;
        private R1230cdugbNode[] _displayNodes;
        private byte[] _srValues;

        /// <summary>
        /// Initialize a new R1230cdugb connected through a shift register.
        /// </summary>
        public R1230cdugb(ShiftRegister shiftRegister, sbyte leds = 12)
        {
            _sr = shiftRegister;
            _cc = new int[] { 13, 19, 26 };
            _controller = new GpioController();
            _leds = new R1230cdugbLed[leds];
            Length = leds;
            _displayNodes = GetNodes();
            _srValues = GetShiftRegisterValues();

            foreach (int pin in _cc)
            {
                _controller.OpenPin(pin, PinMode.Output);
                _controller.Write(pin, 0);
                _controller.SetPinMode(pin, PinMode.Input);
            }
        }

        /// <summary>
        /// The length of the segment; the number of GPIO pins it exposes.
        /// Default is 12 LEDs.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Default color for R1230cdugb.
        /// Valid values are 0 (red), 1 (green), 2 (orange)
        /// Set to 0 initally.
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// Write value (low or high) to LED.
        /// Uses default color.
        /// </summary>
        public void Write(int led, PinValue value, CancellationToken token = default(CancellationToken), int duration = -1)
        {
            Write(led, value, Color, token, duration);
        }

        /// <summary>
        /// Write value (low or high) to LED.
        /// Supports setting one of three colors.
        /// </summary>
        public void Write(int led, PinValue value, int color, CancellationToken token = default(CancellationToken), int duration = -1)
        {
            _leds[led].Value = value;
            _leds[led].Color = color;

            if (duration > -1)
            {
                Display(token, duration);
            }
        }

        /// <summary>
        /// Writes a byte using the default color.
        /// </summary>
        public void Write(byte value, bool shiftValues = false, CancellationToken token = default(CancellationToken), int duration = -1)
        {
            if (Length < 8)
            {
                throw new Exception($"{nameof(Write)}: At least 8 pins must be used to write a byte value.");
            }

            for (ushort i = 0; i < 8; i++)
            {
                // 0b_1000_0000 (same as integer 128) used as input to create mask
                // determines value of i bit in byte value
                // logical equivalent of value[i] (which isn't supported for byte type in C#)
                // starts left-most and ends up right-most
                PinValue pinValue = (0b_1000_0000 >> i) & value;
                Write(i, pinValue);
            }

            if (duration > -1)
            {
                Display(token, duration);
            }
        }

        /// <summary>
        /// Displays segment per the cancellation token (duration or signal).
        /// As appropriate for a given implementation, performs a latch.
        /// </summary>
        public void Display(CancellationToken token = default(CancellationToken), int duration = -1)
        {
            if (duration > -1)
            {
               var linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationTokenSource(duration).Token);
               token = linkedTokens.Token;
            }

            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i < Length; i++)
                {
                    var node = _displayNodes[i];
                    var led = _leds[i];
                    var green = _srValues[node.GreenAnode];
                    var red = _srValues[node.RedAnode];

                    _controller.SetPinMode(_cc[node.Cathode], PinMode.Output);

                    if (led.Value == 0)
                    {
                    }
                    else if (led.Color == 2)
                    {
                        _sr.Write(red, false, token, duration);
                        Thread.Sleep(0);
                        _sr.Write(green, false, token, duration);

                    }
                    else if (led.Color == 1)
                    {
                        _sr.Write(green, false, token, duration);
                    }
                    else
                    {
                        _sr.Write(red, false, token, duration);
                    }

                    Thread.Sleep(1);

                    if ((i + 1) % 4 == 0)
                    {
                        _controller.SetPinMode(_cc[node.Cathode], PinMode.Input);
                    }
                }
            }
        }

        /// <summary>
        /// Provides the set of LED nodes with color information.
        /// Represents 12 nodes, even if multiple 12 LED segments are used.
        /// The wiring is the same for the first 12, the second 12, ...
        /// </summary>
        private R1230cdugbNode[] GetNodes()
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
            return new R1230cdugbNode[]
            {
                new R1230cdugbNode(1, 13, 0),
                new R1230cdugbNode(2, 12, 0),
                new R1230cdugbNode(3, 11, 0),
                new R1230cdugbNode(4, 10, 0),
                new R1230cdugbNode(1, 13, 1),
                new R1230cdugbNode(2, 12, 1),
                new R1230cdugbNode(3, 11, 1),
                new R1230cdugbNode(4, 10, 1),
                new R1230cdugbNode(1, 13, 2),
                new R1230cdugbNode(2, 12, 2),
                new R1230cdugbNode(3, 11, 2),
                new R1230cdugbNode(4, 10, 2),
            };
        }

        private byte[] GetShiftRegisterValues()
        {
            var values = new byte[14];

            values[1] = 0b_1;
            values[13] = 0b_01;
            values[2] = 0b_001;
            values[12] = 0b_0001;
            values[3] = 0b_0000_1;
            values[11] = 0b_0000_01;
            values[4] = 0b_0000_001;
            values[10] = 0b_0000_0001;

            return values;
        }
    }
}
