﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Device.Spi;

namespace Iot.Device.Multiplexing
{
    /// <summary>
    /// Represents pin bindings for the Sn74hc595.
    /// </summary>
    public struct Sn74hc595PinMapping
    {
        /// <param name="ser">Data pin</param>
        /// <param name="oe">Output enable pin</param>
        /// <param name="rclk">Register clock pin (latch)</param>
        /// <param name="srclk">Shift register pin (shift to data register)</param>
        /// <param name="srclr">Shift register clear pin (shift register is cleared)</param>
        public Sn74hc595PinMapping(int ser, int srclk, int rclk, int oe = 0, int srclr = 0)
        {
            Ser = ser;
            SrClk = srclk;
            RClk = rclk;
            OE = oe;
            SrClr = srclr;
        }

        /// <summary>
        /// Standard pin bindings for the Sn74hc595.
        /// </summary>
        public static Sn74hc595PinMapping Standard => new Sn74hc595PinMapping(16, 20, 21, 12, 25);
        /*
            Ser     = 16    // data
            SrClk   = 20    // storage register clock
            RClk    = 21    // latch / publish storage register
            OE      = 12    // blank
            SrClr   = 25    // clear
        */

        /// <summary>
        /// SER (data) pin number.
        /// </summary>
        public int Ser { get; set; }

        /// <summary>
        /// SRCLK (shift) pin number.
        /// </summary>
        public int SrClk { get; set; }

        /// <summary>
        /// RCLK (latch) pin number.
        /// </summary>
        public int RClk { get; set; }

        /// <summary>
        /// OE (output enable) pin number.
        /// </summary>
        public int OE { get; set; }

        /// <summary>
        /// SRCLR (clear register) pin number.
        /// </summary>
        public int SrClr { get; set; }
    }
}
