using System;
using System.Threading;
using Iot.Device.Multiplexing;

/// <summary>
/// Extension methods for IOutputSegment
/// </summary>
public static class IOutputSegmentExtensions
{
    /// <summary>
    /// Extension methods for IOutputSegment
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
