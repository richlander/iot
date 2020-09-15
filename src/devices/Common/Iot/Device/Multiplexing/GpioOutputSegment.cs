// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace Iot.Device.Multiplexing
{
    /// <summary>
    /// IOutputSegment implementation that uses GpioController.
    /// </summary>
    public class GpioOutputSegment : IOutputSegment
    {
        private readonly int[] _pins;
        private readonly bool _shouldDispose;
        private GpioController _controller;

        /// <summary>
        /// IOutputSegment implementation that uses GpioController.
        /// </summary>
        /// <param name="pins">The GPIO pins that should be used and are connected.</param>
        /// <param name="gpioController">The GpioController to use. If one isn't provided, one will be created.</param>
        /// <param name="shouldDispose">The policy to use (true, by default) for disposing the GPIO controller when disposing this instance.</param>
        public GpioOutputSegment(int[] pins, GpioController gpioController = null, bool shouldDispose = true)
        {
            if (gpioController is null)
            {
                gpioController = new GpioController();
            }

            foreach (var pin in pins)
            {
                gpioController.OpenPin(pin, PinMode.Output);
            }

            _pins = pins;
            _controller = gpioController;
            _shouldDispose = shouldDispose;
        }

        /// <summary>
        /// The length of the segment; the number of GPIO pins it exposes.
        /// </summary>
        public int Length => _pins.Length;

        /// <summary>
        /// Writes a PinValue to the underlying GpioController.
        /// </summary>
        public void Write(int pin, PinValue value)
        {
            _controller.Write(_pins[pin], value);
        }

        /// <summary>
        /// Writes a byte to the underlying GpioController.
        /// </summary>
        public void Write(byte value, bool shiftValues = false)
        {
            int iterations = 8 > _pins.Length ? 8 : _pins.Length;

            for (int i = 0; i < iterations; i++)
            {
                // 0b_1000_0000 (same as integer 128) used as input to create mask
                // determines value of i bit in byte value
                // logical equivalent of value[i] (which isn't supported for byte type in C#)
                // starts left-most and ends up right-most
                PinValue state = (0b_1000_0000 >> i) & value;
                Write(i, state);
            }
        }

        /// <summary>
        /// Displays segment for a given duration.
        /// Alternative to Thread.Sleep
        /// </summary>
        public void Display(CancellationToken token)
        {
            token.WaitHandle.WaitOne();
        }

        /// <summary>
        /// Disposes the underlying GpioController.
        /// </summary>
        public void Dispose()
        {
            if (_shouldDispose)
            {
                _controller?.Dispose();
                _controller = null;
            }
        }
    }
}
