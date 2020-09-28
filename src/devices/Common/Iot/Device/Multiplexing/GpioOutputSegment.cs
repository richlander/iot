// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Multiplexing.Extensions;

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
            _shouldDispose = shouldDispose || (gpioController == null);
            _controller = gpioController ?? new GpioController();
            _pins = pins;

            foreach (var pin in pins)
            {
                _controller.OpenPin(pin, PinMode.Output);
            }
        }

        /// <summary>
        /// The length of the segment; the number of GPIO pins it exposes.
        /// </summary>
        public int Length => _pins.Length;

        /// <summary>
        /// Writes a PinValue to the underlying GpioController.
        /// </summary>
        public void Write(int output, PinValue value, CancellationToken token = default(CancellationToken), int duration = -1)
        {
            _controller.Write(_pins[output], value);

            if (duration > -1)
            {
                Display(token, duration);
            }
        }

        /// <summary>
        /// Writes a byte to the underlying GpioController.
        /// </summary>
        public void Write(byte value, bool shiftValues = false, CancellationToken token = default(CancellationToken), int duration = -1)
        {
            if (_pins.Length < 8)
            {
                throw new Exception($"{nameof(Write)}: At least 8 pins must be used to write a byte value.");
            }

            for (int i = 0; i < 8; i++)
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
        /// Displays segment until a token is cancelled, possibly due to a duration expiring.
        /// Alternative to Thread.Sleep
        /// </summary>
        public void Display(CancellationToken token = default(CancellationToken), int duration = -1)
        {
            WaitOnTokenOrDuration(token, duration);
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

        /// <summary>
        /// Wait on CancellationToken or duration IOutputSegment.
        /// Utility method intended to be called from the Display method of the IOutputSegment interface.
        /// </summary>
        public static void WaitOnTokenOrDuration(CancellationToken token = default(CancellationToken), int duration = -1)
        {
            if (duration > -1)
            {
                CancellationTokenSource linkedTokens = CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationTokenSource(duration).Token);
                linkedTokens.Token.WaitHandle.WaitOne();
                return;
            }
            else if (token == default(CancellationToken))
            {
                return;
            }

            token.WaitHandle.WaitOne();
        }
    }
}
