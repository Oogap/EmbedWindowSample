using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace EmbedWindowSample;

/// <summary>
/// 用于在 WPF 应用程序中嵌入外部窗口的宿主类。
/// 继承自 <see cref="HwndHost"/>，提供对外部窗口的管理功能。
/// </summary>
public class EmbeddedWindowHost : HwndHost
{
    private readonly IntPtr _externalHwnd; // 外部窗口的句柄
    private IntPtr _hostHwnd; // 宿主窗口的句柄

    /// <summary>
    /// 构造函数，初始化 <see cref="EmbeddedWindowHost"/> 类的实例。
    /// </summary>
    /// <param name="externalHwnd">要嵌入的外部窗口的句柄。</param>
    public EmbeddedWindowHost(IntPtr externalHwnd)
    {
        _externalHwnd = externalHwnd;
    }

    /// <summary>
    /// 创建宿主窗口的核心方法。
    /// </summary>
    /// <param name="hwndParent">父窗口的句柄引用。</param>
    /// <returns>返回创建的宿主窗口的句柄引用。</returns>
    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        int width = (int)ActualWidth;
        int height = (int)ActualHeight;

        // 创建宿主窗口
        _hostHwnd = WindowsAPI.CreateWindowEx(
            0,
            "static",
            "",
            WindowStyleFlags.WS_CHILD | WindowStyleFlags.WS_VISIBLE,
            0,
            0,
            width,
            height,
            hwndParent.Handle,
            IntPtr.Zero,
            WindowsAPI.GetModuleHandle(null!),
            IntPtr.Zero);

        // 设置外部窗口的父窗口为宿主窗口
        WindowsAPI.SetParent(_externalHwnd, _hostHwnd);

        // 修改外部窗口的样式，使其成为子窗口
        long style = WindowsAPI.GetWindowLong(_externalHwnd, GetWindowLongIndexes.GWL_STYLE);
        style &= ~(WindowStyleFlags.WS_POPUP);
        style |= WindowStyleFlags.WS_CHILD;
        WindowsAPI.SetWindowLong(_externalHwnd, GetWindowLongIndexes.GWL_STYLE, style);

        // 初始调整外部窗口的大小
        UpdateWindowSize(_externalHwnd, width, height);

        return new HandleRef(this, _hostHwnd);
    }

    /// <summary>
    /// 销毁宿主窗口的核心方法。
    /// </summary>
    /// <param name="hwnd">宿主窗口的句柄引用。</param>
    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        // 如果外部窗口存在，发送关闭消息
        if (_externalHwnd != IntPtr.Zero)
        {
            WindowsAPI.SendMessage(_externalHwnd, WindowMessages.WM_CLOSE, 0, 0);
        }

        // 销毁宿主窗口
        if (_hostHwnd != IntPtr.Zero)
        {
            WindowsAPI.DestroyWindow(_hostHwnd);
            _hostHwnd = IntPtr.Zero;
        }
    }

    /// <summary>
    /// 处理宿主窗口大小变化的事件。
    /// </summary>
    /// <param name="sizeInfo">包含大小变化信息的 <see cref="SizeChangedInfo"/>。</param>
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        if (_hostHwnd == IntPtr.Zero) return;

        int width = (int)sizeInfo.NewSize.Width;
        int height = (int)sizeInfo.NewSize.Height;

        // 更新宿主窗口的大小
        UpdateWindowSize(_hostHwnd, width, height);

        // 更新外部窗口的大小
        UpdateWindowSize(_externalHwnd, width, height);
    }

    /// <summary>
    /// 更新窗口的大小。
    /// </summary>
    /// <param name="width">新的宽度。</param>
    /// <param name="height">新的高度。</param>
    private void UpdateWindowSize(IntPtr hwnd, int width, int height)
    {
        if (hwnd == IntPtr.Zero) return;

        // 设置外部窗口的位置和大小
        WindowsAPI.SetWindowPos(
            hwnd,
            IntPtr.Zero,
            0,
            0,
            width,
            height,
            SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

        // 强制窗口重绘
        WindowsAPI.InvalidateRect(hwnd, IntPtr.Zero, true);
        WindowsAPI.UpdateWindow(hwnd);
    }

    /// <summary>
    /// 恢复外部窗口的样式。
    /// </summary>
    /// <param name="externalHwnd">外部窗口的句柄。</param>
    private void RestoreExternalWindowStyle(IntPtr externalHwnd)
    {
        long style = WindowsAPI.GetWindowLong(externalHwnd, GetWindowLongIndexes.GWL_STYLE);
        style &= ~WindowStyleFlags.WS_CHILD;
        style |= WindowStyleFlags.WS_POPUP;
        WindowsAPI.SetWindowLong(externalHwnd, GetWindowLongIndexes.GWL_STYLE, style);
    }
}
