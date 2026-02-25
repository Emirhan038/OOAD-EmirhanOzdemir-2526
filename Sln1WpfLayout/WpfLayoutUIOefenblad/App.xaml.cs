using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace WpfLayoutUIOefenblad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BrowserEmulation.SetIe11ModeForCurrentExe();
        }

        private static class BrowserEmulation
        {
            // Forces the WPF WebBrowser control to use IE11 document mode (Edge mode of IE11).
            // Without this, it may fall back to older compatibility modes on some machines.
            public static void SetIe11ModeForCurrentExe()
            {
                try
                {
                    string exeName = GetCurrentProcessExeFileName();
                    if (string.IsNullOrWhiteSpace(exeName))
                    {
                        return;
                    }

                    const string keyPath =
                        @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

                    using RegistryKey? key = Registry.CurrentUser.CreateSubKey(keyPath);
                    if (key == null)
                    {
                        return;
                    }

                    // 11001 = IE11 Edge mode
                    key.SetValue(exeName, 11001, RegistryValueKind.DWord);
                }
                catch
                {
                    // Intentionally swallow: app should keep working even if we can't write the registry.
                    // (e.g., restricted environment, locked registry, etc.)
                }
            }

            private static string GetCurrentProcessExeFileName()
            {
                try
                {
                    using Process current = Process.GetCurrentProcess();
                    string? fullPath = current.MainModule?.FileName;
                    if (string.IsNullOrWhiteSpace(fullPath))
                    {
                        return string.Empty;
                    }

                    return Path.GetFileName(fullPath);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }

}
