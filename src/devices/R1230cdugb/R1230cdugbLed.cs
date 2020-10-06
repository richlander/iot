// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Device.Gpio;

namespace Iot.Device.Multiplexing
{
    /// <summary>
    /// Represents an LED in one or more KWL-R1230CDUGB 12 segment light bar display units.
    /// </summary>
    public struct R1230cdugbLed
    {
        /// <summary>
        /// Value of LED.
        /// Range: 0-1.
        /// </summary>
        public PinValue Value;

        /// <summary>
        /// Color LED.
        /// Range: 0-2.
        /// </summary>
        public int Color;
    }
}
