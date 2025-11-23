using Windows.Win32;
using WinWrapper;
using WinWrapper.Windowing;

System.Threading.Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
System.Threading.Thread.CurrentThread.SetApartmentState(ApartmentState.STA);


Window wind = default;

WindowClass windowClass = new("SimplyTools", WindowProc, WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW);

wind = Window.CreateNewWindow("SimplyTools.NoWPF", windowClass);
wind.Show();
wind.Update();
wind.SendMessage(WindowMessages.USER, 0, 0);
wind[WindowExStyles.COMPOSITED] = true;

try
{
#pragma warning disable CA1416 // Validate platform compatibility
    wind.DwmAttribute.SystemBackdrop = WinWrapper.Windowing.Dwm.SystemBackdropTypes.MainWindow;
#pragma warning restore CA1416 // Validate platform compatibility
    wind.ExtendFrameIntoClientArea(new(-1,-1,-1,-1));
}
catch
{
    // system backdrop not supported
}

Application.RunMessageLoopOnCurrentThread();

nint WindowProc(Window window, WindowMessages WindowMessage, nuint wParam, nint lParam)
{
    switch (WindowMessage)
    {
        case WindowMessages.Destroy:
            PInvoke.PostQuitMessage(0);
            break;
        case WindowMessages.PAINT:
            var hDC = PInvoke.BeginPaint(new(window.Handle), out var lpPaint);
            PInvoke.EndPaint(new(window.Handle), in lpPaint);
            break;
        //case WindowMessages.SIZE:
        //    UpdateWebViewBounds();
        //    break;
        case WindowMessages.EarseBackground:
            return 1;
        case WindowMessages.USER:
            //Init();
            break;
        default:
            return window.DefWindowProc(WindowMessage, wParam, lParam);
    }
    return 0;
}