# EmbedWindowSample

## 项目简介
本演示程序展示两种在WPF应用中嵌入外部窗口的方法：
1. 使用HwndSource创建托管窗口
2. 直接调用Windows API创建原生窗口

用户可通过输入目标窗口标题，选择不同嵌入方式将外部应用程序窗口嵌入到当前应用的容器区域。

## 运行环境
- Windows 11
- .NET 6

## 使用说明
1. 克隆/下载项目仓库
2. 使用Visual Studio打开解决方案文件
3. 编译并运行项目
4. 操作步骤：
   - 启动目标嵌入程序（如notepad.exe）
   - 在输入框输入窗口标题（如"无标题 - Notepad"）
   - 点击任一嵌入按钮
   - 观察下方容器区域的嵌入效果
   - 关闭窗口自动关闭嵌入窗口

## 实现原理
### HwndSource方式
```csharp
// 创建托管窗口
var parameters = new HwndSourceParameters("EmbedWindow")
{
    ParentWindow = containerHwnd,
    WindowStyle = WindowStyleFlags.WS_CHILD // 设置窗口样式为子窗口
    // 设置尺寸
};
var hwndSource = new HwndSource(parameters);
```

### Windows API方式
```csharp
// 使用Win32 API
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

// 创建原生窗口
IntPtr hWnd = CreateWindowEx(
    0,                                // 扩展样式
    "static",                         // 预定义的窗口类，更多自定义需注册窗口类
    "",                               // 窗口标题
    WS_OVERLAPPEDWINDOW | WS_VISIBLE, // 窗口样式
    100,                              // X 坐标
    100,                              // Y 坐标
    800,                              // 宽度
    600,                              // 高度
    IntPtr.Zero,                      // 父窗口句柄
    IntPtr.Zero,                      // 菜单句柄
    GetModuleHandle(null!),           // 实例句柄
    IntPtr.Zero);                     // 创建参数

```

## 目录结构
```
/EmbedWindowSample
├── MainWindow.xaml        # 主界面定义
├── MainWindow.xaml.cs     # 主逻辑代码
├── WindowsAPI.cs          # Win32 API声明
├── EmbeddedWindowHost.cs  # 使用 Window API 创建窗口并嵌入
├── ExternalWindowHost.cs  # 使用 HwndSource 创建窗口并嵌入
└── App.xaml               # 应用配置
```

## 注意事项
1. 确保输入正确的窗口标题
2. 可借助Spy++工具查找窗口标题
3. 主程序关闭后自动关闭嵌入的子窗口

## 常见问题
Q: 无法找到目标窗口？</br>
A: 1. 确认标题完全匹配 2. 使用Spy++验证窗口句柄

## 贡献
欢迎提交Issue或Pull Request

## 许可证
MIT License
