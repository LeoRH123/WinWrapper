using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Graphics.Dwm;
using WinWrapper.Windowing.Dwm;

namespace WinWrapper.Windowing;
public interface IDwmWindowAttribute
{
    T Get<T>(DwmWindowAttribute dwAttribute) where T : unmanaged;
    void Set<T>(DwmWindowAttribute dwAttribute, T value) where T : unmanaged;

#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows10.22621.0.0")]
#endif
    SystemBackdropTypes SystemBackdrop { get; set; }
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows10.22000.0.0")]
#endif
    WindowCornerPreferences WindowCornerPreference { get; set; }
}
public partial struct Window : IDwmWindowAttribute
{
    public IDwmWindowAttribute DwmAttribute => this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly unsafe T IDwmWindowAttribute.Get<T>(DwmWindowAttribute dwAttribute)
    {
        T ToReturn = new();
        PInvoke.DwmGetWindowAttribute(HWND, (DWMWINDOWATTRIBUTE)dwAttribute, &ToReturn, (uint)sizeof(T));
        return ToReturn;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly unsafe void IDwmWindowAttribute.Set<T>(DwmWindowAttribute dwAttribute, T value)
    {
        PInvoke.DwmSetWindowAttribute(HWND, (DWMWINDOWATTRIBUTE)dwAttribute, &value, (uint)sizeof(T));
    }
    SystemBackdropTypes IDwmWindowAttribute.SystemBackdrop
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => DwmAttribute.Get<SystemBackdropTypes>(DwmWindowAttribute.SystemBackdropTypes);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => DwmAttribute.Set(DwmWindowAttribute.SystemBackdropTypes, value);
    }
    WindowCornerPreferences IDwmWindowAttribute.WindowCornerPreference
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => DwmAttribute.Get<WindowCornerPreferences>(DwmWindowAttribute.WindowCornderPreference);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => DwmAttribute.Set(DwmWindowAttribute.WindowCornderPreference, value);
    }
}

