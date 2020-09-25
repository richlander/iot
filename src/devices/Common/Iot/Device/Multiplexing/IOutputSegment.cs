// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Threading;

namespace Iot.Device.Multiplexing
{
    /// <summary>
    /// Interface that abstracts multiplexing over a segment of outputs.
    /// </summary>
    public interface IOutputSegment
    {
        /// <summary>
        /// Length of segment (number of outputs)
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Writes a PinValue to a multiplexed output.
        /// Does not perform a latch, for implementations where that is relevant.
        /// Will latch if a duration is specified.
        /// </summary>
        void Write(int output, PinValue value, CancellationToken token = default(CancellationToken), int duration = -1);

        /// <summary>
        /// Writes a byte to a multiplexed output.
        /// Does not perform a latch, for implementations where that is relevant.
        /// Will latch if a duration is specified.
        /// </summary>
        void Write(byte value, bool shiftValues = false, CancellationToken token = default(CancellationToken), int duration = -1);

        /// <summary>
        /// Displays segment until token receives a cancellation signal, possibly due to a specificated duration.
        /// As appropriate for a given implementation, performs a latch.
        /// </summary>
        void Display(CancellationToken token = default(CancellationToken), int duration = -1);
    }
}
