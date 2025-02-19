using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace EmbedWindowSample;

/// <summary>
/// 用于在 WPF 应用程序中嵌入外部窗口的宿主类。
/// 继承自 <see cref="HwndHost"/>，提供对外部窗口的管理功能。
/// </summary>
public class ExternalWindowHost : HwndHost
{
    private IntPtr _externalHwnd; // 外部窗口的句柄
    private HwndSource _hostHwndSource = null!; // 宿主窗口的 HwndSource 对象

    /// <summary>
    /// 创建宿主窗口的核心方法。
    /// </summary>
    /// <param name="hwndParent">父窗口的句柄引用。</param>
    /// <returns>返回创建的宿主窗口的句柄引用。</returns>
    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        // 配置宿主窗口的参数
        var parameters = new HwndSourceParameters("ExternalHost")
        {
            ParentWindow = hwndParent.Handle, // 设置父窗口
            Width = (int)(ActualWidth + 0.5), // 设置宽度
            Height = (int)(ActualHeight + 0.5), // 设置高度
            WindowStyle = WindowStyleFlags.WS_CHILD // 设置窗口样式为子窗口
        };

        // 创建宿主窗口
        _hostHwndSource = new HwndSource(parameters);
        return new HandleRef(this, _hostHwndSource.Handle);
    }

    /// <summary>
    /// 将外部窗口附加到宿主窗口中。
    /// </summary>
    /// <param name="hwnd">外部窗口的句柄。</param>
    /// <returns>如果附加成功，返回 true；否则返回 false。</returns>
    public bool AttachToHwnd(IntPtr hwnd)
    {
        _externalHwnd = hwnd;
        if (_externalHwnd == IntPtr.Zero) return false;

        // 修改外部窗口的样式，使其成为子窗口
        var style = WindowsAPI.GetWindowLong(_externalHwnd, GetWindowLongIndexes.GWL_STYLE);
        WindowsAPI.SetWindowLong(_externalHwnd, GetWindowLongIndexes.GWL_STYLE, (style & ~WindowStyleFlags.WS_POPUP) | WindowStyleFlags.WS_CHILD);

        // 设置外部窗口的父窗口为宿主窗口
        WindowsAPI.SetParent(_externalHwnd, _hostHwndSource!.Handle);

        // 更新外部窗口的大小以匹配宿主窗口
        UpdateExternalWindowSize();

        // 订阅 SizeChanged 事件，以便在宿主窗口大小变化时调整外部窗口的大小
        SizeChanged += ExternalWindowHost_SizeChanged;
        return true;
    }

    /// <summary>
    /// 更新外部窗口的大小以匹配宿主窗口的大小。
    /// </summary>
    private void UpdateExternalWindowSize()
    {
        if (_externalHwnd == IntPtr.Zero) return;

        // 设置外部窗口的位置和大小
        WindowsAPI.SetWindowPos(
            _externalHwnd, IntPtr.Zero,
            0, 0,
            (int)(ActualWidth + 0.5), (int)(ActualHeight + 0.5),
            SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);
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
        WindowsAPI.DestroyWindow(_hostHwndSource.Handle);
        _hostHwndSource?.Dispose(); // 释放资源
    }

    /// <summary>
    /// 处理宿主窗口大小变化的事件。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">包含事件数据的 <see cref="SizeChangedEventArgs"/>。</param>
    private void ExternalWindowHost_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateExternalWindowSize(); // 更新外部窗口的大小
    }
}
