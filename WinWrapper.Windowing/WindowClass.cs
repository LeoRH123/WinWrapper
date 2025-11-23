using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
namespace WinWrapper.Windowing;

public readonly struct WindowClass
{
    public unsafe delegate nint WindowProc(Window hWnd, WindowMessages WindowMessage, nuint wParam, nint lParam);
    readonly WNDPROC? WndProc = default;
    public readonly string Name;
    private WindowClass(string ClassName, bool _) { this.Name = ClassName; }
    public unsafe WindowClass(
        string ClassName,
        WindowProc? WndProc = null,
        WindowClassStyles ClassStyle = default,
        nint? BackgroundBrush = default
    ) : this(ClassName, WndProc, ClassStyle, BackgroundBrush.HasValue ? new HBRUSH(BackgroundBrush.Value) : null) { }
    internal unsafe WindowClass(
        string ClassName,
        WindowProc? WndProc = null,
        WindowClassStyles ClassStyle = default,
        HBRUSH? BackgroundBrush = null
    )
    {
        this.Name = ClassName;
        if (WndProc != null)
            this.WndProc = (hWnd, WM, WP, LP) => new(WndProc.Invoke(
                Window.FromWindowHandle(hWnd),
                (WindowMessages)WM,
                WP,
                LP
            ));
        this.WndProc ??= DefaultWndProc;
        var hInstance = PInvoke.GetModuleHandle(default(PCWSTR));
        fixed (char* className = ClassName)
        {
            WNDCLASSW cls = new()
            {
                lpszClassName = className,
                hInstance = hInstance,
                lpfnWndProc = this.WndProc,
                style = (WNDCLASS_STYLES)ClassStyle,
                hbrBackground = BackgroundBrush.HasValue ? BackgroundBrush.Value : new((nint)SYS_COLOR_INDEX.COLOR_WINDOW + 1),
                cbWndExtra = sizeof(nint),
                hCursor = PInvoke.LoadCursor(HINSTANCE.Null, (char*)PInvoke.IDC_ARROW),
            };
            PInvoke.RegisterClass(cls);
        }
    }
    public static WindowClass FromExistingClass(string ClassName) => new(ClassName, false);


    unsafe static LRESULT DefaultWndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case PInvoke.WM_DESTROY:
                PInvoke.PostQuitMessage(0);
                return default;

            case PInvoke.WM_PAINT:
                var hdc = PInvoke.BeginPaint(hWnd, out var ps);
                PInvoke.EndPaint(hWnd, ps);
                return default;
        }

        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }
}
