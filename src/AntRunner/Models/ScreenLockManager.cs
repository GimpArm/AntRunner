using System;
using System.Runtime.InteropServices;

namespace AntRunner.Models
{
    public static class ScreenLockManager
    {
        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            // Legacy flag, should not be used.
            // ES_USER_PRESENT   = 0x00000004,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public static void DisableSleep()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                                    | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                                    | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                                    | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        public static void EnableSleep()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }
    }
}
