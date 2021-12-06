using System.Runtime.InteropServices;

namespace HI.DevOps.DomainCore.Helper
{
    public static class OSHelper
    {
        public static bool IsWin()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsMac()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public static bool IsGnu()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static string GetCurrent()
        {
            return
                (IsWin() ? "win" : null) ??
                (IsMac() ? "mac" : null) ??
                (IsGnu() ? "gnu" : null);
        }
    }
}