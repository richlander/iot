using System;
using System.Threading;
using System.Device.Gpio;
using Iot.Device.Multiplexing;

namespace Iot.Device.Multiplexing.Extensions
{
    /// <summary>
    /// Extension methods for IOutputSegment
    /// </summary>
    public static class IOutputSegmentExtensions
    {
        /// <summary>
        /// Clear all outputs in IOutputSegment
        /// </summary>
        public static void Clear(this IOutputSegment segment)
        {
            for (int i = 0; i < segment.Length; i++)
            {
                segment.Write(i, 0);
            }

            segment.Display(new CancellationTokenSource(0).Token);
        }
    }
}
