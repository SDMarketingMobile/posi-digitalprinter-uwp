using System;

namespace POSIDigitalPrinter.Utils
{
    public sealed class WinApplication
    {
        /// <summary>
        /// Check if the current application is running in 32 bits mode.
        /// </summary>
        /// <returns></returns>
        public static bool Is32BitMode()
        {
            return IntPtr.Size == 4;
        }

        /// <summary>
        /// Check if the current application is running in 64 bits mode.
        /// </summary>
        /// <returns></returns>
        public static bool Is64BitMode()
        {
            return IntPtr.Size == 8;
        }
    }
}