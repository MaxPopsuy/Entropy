using Microsoft.Win32;
using static Entropy.Common;

namespace Entropy
{
    internal class AutostartManager
    {
        private const string AppName = EntropyName;
        private static readonly string ExecutablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        public static void EnableAutoStart()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key.SetValue(AppName, ExecutablePath);
            }
        }

        public static void DisableAutoStart()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key.DeleteValue(AppName, false);
            }
        }

        public static bool IsAutoStartEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
            {
                var value = key.GetValue(AppName);
                return value != null && value.ToString() == ExecutablePath;
            }
        }
    }
}
