using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace EmbedWindowSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EmbedButton1_Click(object sender, RoutedEventArgs e)
        {
            var hwnd = WindowsAPI.FindWindow(null!, Input.Text);
            Debug.WriteLine($"Found the window: {hwnd}");

            if (hwnd == IntPtr.Zero) return;

            var host = new EmbeddedWindowHost(hwnd);
            HostGrid.Children.Add(host);

            DisableControl();
        }

        private void EmbedButton2_Click(object sender, RoutedEventArgs e)
        {
            //var whnd = WindowsAPI.FindWindow(null!, "HC-TPS Demo");
            var hwnd = WindowsAPI.FindWindow(null!, Input.Text);
            Debug.WriteLine($"window handle: {hwnd}");

            if (hwnd == IntPtr.Zero) return;

            ExternalHost.AttachToHwnd(hwnd);

            DisableControl();
        }

        private void DisableControl()
        {
            UserInput.IsEnabled = false;
        }
    }
}