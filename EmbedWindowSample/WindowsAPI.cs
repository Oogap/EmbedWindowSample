using System.Runtime.InteropServices;

namespace EmbedWindowSample;

internal static class WindowMessages
{
    // 常用窗口消息
    public const uint WM_CLOSE = 0x0010;              // 窗口关闭时发送
    public const uint WM_COMMAND = 0x0111;            // 用户选择菜单项或控件时发送
    public const uint WM_PAINT = 0x000F;              // 窗口需要重绘时发送
    public const uint WM_SIZE = 0x0005;               // 窗口大小改变时发送
    public const uint WM_MOUSEMOVE = 0x0200;          // 鼠标移动时发送
    public const uint WM_LBUTTONDOWN = 0x0201;        // 鼠标左键按下时发送
    public const uint WM_LBUTTONUP = 0x0202;          // 鼠标左键释放时发送
    public const uint WM_RBUTTONDOWN = 0x0204;        // 鼠标右键按下时发送
    public const uint WM_RBUTTONUP = 0x0205;          // 鼠标右键释放时发送
    public const uint WM_KEYDOWN = 0x0100;            // 键盘按键按下时发送
    public const uint WM_KEYUP = 0x0101;              // 键盘按键释放时发送
    public const uint WM_CHAR = 0x0102;               // 字符输入时发送
    public const uint WM_SETFOCUS = 0x0007;           // 窗口获得焦点时发送
    public const uint WM_KILLFOCUS = 0x0008;          // 窗口失去焦点时发送
    public const uint WM_TIMER = 0x0113;              // 定时器触发时发送
}


internal static class GetWindowLongIndexes
{
    public const int GWL_WNDPROC = -4;       // 获取窗口过程的地址
    public const int GWL_HINSTANCE = -6;     // 获取应用程序实例的句柄
    public const int GWL_HWNDPARENT = -8;    // 获取父窗口的句柄
    public const int GWL_STYLE = -16;        // 获取窗口样式
    public const int GWL_EXSTYLE = -20;      // 获取扩展窗口样式
    public const int GWL_USERDATA = -21;     // 获取与窗口关联的用户数据
    public const int GWL_ID = -12;           // 获取子窗口的标识符
}

internal static class SetWindowPosFlags
{
    public const int SWP_NOSIZE = 0x0001;          // 保持当前大小（忽略 cx 和 cy 参数）
    public const int SWP_NOMOVE = 0x0002;          // 保持当前位置（忽略 x 和 y 参数）
    public const int SWP_NOZORDER = 0x0004;        // 保持当前 Z 顺序（忽略 hWndInsertAfter 参数）
    public const int SWP_NOREDRAW = 0x0008;        // 不重绘窗口
    public const int SWP_NOACTIVATE = 0x0010;      // 不激活窗口
    public const int SWP_FRAMECHANGED = 0x0020;    // 强制发送 WM_NCCALCSIZE 消息，即使窗口大小未改变
    public const int SWP_SHOWWINDOW = 0x0040;      // 显示窗口
    public const int SWP_HIDEWINDOW = 0x0080;      // 隐藏窗口
    public const int SWP_NOCOPYBITS = 0x0100;      // 清除客户区的所有内容
    public const int SWP_NOOWNERZORDER = 0x0200;   // 不改变拥有者窗口的 Z 顺序
    public const int SWP_NOSENDCHANGING = 0x0400;  // 不发送 WM_WINDOWPOSCHANGING 消息
    public const int SWP_DRAWFRAME = 0x0020;       // 绘制窗口边框（与 SWP_FRAMECHANGED 相同）
    public const int SWP_ASYNCWINDOWPOS = 0x4000;  // 异步更新窗口位置
}

internal static class WindowStyleFlags
{
    // 常用窗口样式
    public const int WS_BORDER = 0x00800000;          // 窗口具有细线边框
    public const int WS_CAPTION = 0x00C00000;         // 窗口具有标题栏（包括 WS_BORDER）
    public const int WS_CHILD = 0x40000000;          // 创建一个子窗口（超出 int 范围，保留 uint）
    public const int WS_CLIPCHILDREN = 0x02000000;    // 在父窗口中绘制时，排除子窗口占用的区域
    public const int WS_DISABLED = 0x08000000;        // 创建一个初始禁用的窗口
    public const int WS_DLGFRAME = 0x00400000;        // 创建一个具有对话框边框样式的窗口
    public const int WS_HSCROLL = 0x00100000;         // 窗口具有水平滚动条
    public const int WS_MAXIMIZE = 0x01000000;        // 窗口最初最大化
    public const int WS_MAXIMIZEBOX = 0x00010000;     // 窗口具有最大化按钮
    public const int WS_MINIMIZE = 0x20000000;       // 窗口最初最小化（超出 int 范围，保留 uint）
    public const int WS_MINIMIZEBOX = 0x00020000;     // 窗口具有最小化按钮
    public const int WS_OVERLAPPED = 0x00000000;      // 创建一个重叠窗口
    public const uint WS_POPUP = 0x80000000;          // 创建一个弹出窗口（超出 int 范围，保留 uint）
    public const int WS_SYSMENU = 0x00080000;         // 窗口在标题栏中具有系统菜单框
    public const int WS_THICKFRAME = 0x00040000;      // 窗口具有可调整大小的边框
    public const int WS_VISIBLE = 0x10000000;         // 窗口最初可见
    public const int WS_VSCROLL = 0x00200000;         // 窗口具有垂直滚动条
}

internal static class WindowsAPI
{
    [DllImport("user32.dll ")]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, long dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong)
    {
        return IntPtr.Size == 4 ?
            SetWindowLong32(hWnd, nIndex, (int)dwNewLong) :
            SetWindowLong64(hWnd, nIndex, (IntPtr)dwNewLong);
    }

    [DllImport("user32.dll")]
    public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

    [DllImport("user32.dll")]
    public static extern bool UpdateWindow(IntPtr hWnd);


    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr CreateWindowEx(
        int dwExStyle,
        string lpClassName,
        string lpWindowName,
        int dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

}
