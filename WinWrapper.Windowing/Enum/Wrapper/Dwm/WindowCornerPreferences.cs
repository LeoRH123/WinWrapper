using Windows.Win32.Graphics.Dwm;
namespace WinWrapper.Windowing.Dwm;

public enum WindowCornerPreferences
{
    RoundSmall = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL,
    Round = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND,
    DoNotRound = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND,
    Default = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT
}