using System;

namespace POSIDigitalPrinter.Utils
{
    public static class WindowsVersiosExtension
    {
        /// <summary>
        /// Verifica se a versão atual é maior ou igual a versão do windows 8.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsVersionGreaterOrEqualsToWin8(this WinVersion.WindowsVersions version)
        {
            switch (version)
            {
                case WinVersion.WindowsVersions.Win8:
                case WinVersion.WindowsVersions.Win81:
                case WinVersion.WindowsVersions.Win10:
                    return true;
                default:
                    return false;
            }
        }
    }

    public sealed class WinVersion
    {
        public enum WindowsVersions { UnKnown, Win95, Win98, WinMe, WinNT3, WinNT4, Win2000, WinXP, WinServer2003, WinVista, Win7, Win8, Win81, Win10, MacOSX, Unix, Xbox };
        
        /// <summary>
        /// Get Operational System version.
        /// </summary>
        /// <returns></returns>
        public static WindowsVersions GetCurrentWindowsVersion()
        {
            // Get OperatingSystem information from the system namespace.
            OperatingSystem osInfo = Environment.OSVersion;

            // Determine the platform.
            if (osInfo.Platform == PlatformID.Win32Windows)
            {
                // Platform is Windows 95, Windows 98, Windows 98 Second Edition, or Windows Me.
                switch (osInfo.Version.Minor)
                {
                    case 0:
                        return WindowsVersions.Win95;
                    case 10:
                        return WindowsVersions.Win98;
                    case 90:
                        return WindowsVersions.WinMe;
                }
            }
            else if (osInfo.Platform == PlatformID.Win32NT)
            {
                switch (osInfo.Version.Major)
                {
                    case 3:
                        return WindowsVersions.WinNT3;
                    case 4:
                        return WindowsVersions.WinNT4;
                    case 5:
                        switch (osInfo.Version.Minor)
                        {
                            case 0:
                                return WindowsVersions.Win2000;
                            case 1:
                                return WindowsVersions.WinXP;
                            case 2:
                                return WindowsVersions.WinServer2003;
                        }
                        break;
                    case 6:
                        switch (osInfo.Version.Minor)
                        {
                            case 0:
                                return WindowsVersions.WinVista;
                            case 1:
                                return WindowsVersions.Win7;
                            case 2:
                                return WindowsVersions.Win8;
                            default:
                                return WindowsVersions.Win81;
                        }
                    case 10:
                        return WindowsVersions.Win10;
                }
            }
            else if (osInfo.Platform == PlatformID.Unix)
            {
                return WindowsVersions.Unix;
            }
            else if (osInfo.Platform == PlatformID.MacOSX)
            {
                return WindowsVersions.MacOSX;
            }
            else if (osInfo.Platform == PlatformID.Xbox)
            {
                return WindowsVersions.Xbox;
            }
            return WindowsVersions.UnKnown;
        }

        /// <summary>
        /// Check if current Operational System is 64bits.
        /// </summary>
        /// <returns></returns>
        public static bool Is64Bits()
        {
            return Environment.Is64BitOperatingSystem;
        }
    }
}