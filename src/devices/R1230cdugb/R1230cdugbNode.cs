// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Device.Gpio;

namespace Iot.Device.Multiplexing
{
    /// <summary>
    /// Pattern for a lighting an LED in a KWL-R1230CDUGB 12 segment light bar display.
    /// </summary>
    public struct R1230cdugbNode
    {
        /// <summary>
        /// Instantiate a R1230cdugbNode.
        /// </summary>
        public R1230cdugbNode(int redAnode, int greenAnode, int cathode)
        {
            RedAnode = redAnode;
            GreenAnode = greenAnode;
            Cathode = cathode;
        }

        /// <summary>
        /// Red anode leg (power) for LED.
        /// </summary>
        public int RedAnode;

        /// <summary>
        /// Green anode leg (power) for LED.
        /// </summary>
        public int GreenAnode;

        /// <summary>
        /// Cathode leg (ground) for LED.
        /// Range: 0-2. Intended to be used as an index into an array.
        /// </summary>
        public int Cathode;
    }
}