﻿using System.Drawing;
using System.Runtime.CompilerServices;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Shell.Common;
using System.Runtime.Versioning;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
namespace WinWrapper;

public partial struct Display
{
    public readonly HMONITOR Handle;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Display(HMONITOR HMonitor)
    {
        Handle = HMonitor;
    }
    /// <summary>
    /// Returns the <see cref="Display"/> is NULL
    /// </summary>
    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Handle.Value != 0;
    }
    public override string ToString()
        => IsNull ? $"{nameof(Display)} {Handle.Value}" : $"Invalid {nameof(Display)} ({Handle.Value})";
    /// <summary>
    /// Returns the scale factor of the <see cref="Display"/>
    /// </summary>
    [SupportedOSPlatform("windows8.1")]
    public int ScaleFactor
    {
        get
        {
            PInvoke.GetScaleFactorForMonitor(Handle, out var scaleFactor).ThrowOnFailure();
            if (scaleFactor == 0) scaleFactor = DEVICE_SCALE_FACTOR.SCALE_100_PERCENT;
            return (int)scaleFactor;
        }
    }

    public MONITORINFO MonitorInfo
    {
        get
        {
            MONITORINFO m;
            unsafe
            {
                m = new() { cbSize = (uint)sizeof(MONITORINFO), dwFlags = (uint)MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST };
            }
            PInvoke.GetMonitorInfo(Handle, ref m).ThrowOnFailure();
            return m;
        }
    }
    /// <summary>
    /// Returns the working area bounds of the <see cref="Display"/>
    /// </summary>
    public Rectangle WorkingAreaBounds
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MonitorInfo.rcWork;
    }
    /// <summary>
    /// Returns the bounds of the <see cref="Display"/>
    /// </summary>
    public Rectangle MonitorBounds
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MonitorInfo.rcMonitor;
    }
    /// <summary>
    /// Returns if the <see cref="Display"/> is a primary screen
    /// </summary>
    public bool IsPrimary
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (MonitorInfo.dwFlags & PInvoke.MONITORINFOF_PRIMARY) != 0;
    }

    public unsafe void SetWorkingArea(int top, int left, int right, int bottom)
    {
        var bounds = MonitorBounds;

        RECT workArea = new()
        {
            top = bounds.Top + top,
            left = bounds.Left + left,
            right = bounds.Right - right,
            bottom = bounds.Bottom - bottom
        };

        //Probably will need rework when using more than 1 monitor
        PInvoke.SystemParametersInfo(
            SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETWORKAREA,
            0,
            &workArea,
            SYSTEM_PARAMETERS_INFO_UPDATE_FLAGS.SPIF_UPDATEINIFILE
        );
    }
}