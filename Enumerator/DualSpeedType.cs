namespace POSIDigitalPrinter.Enumerator
{
    /// <summary>
    /// Velocidades das impressoras.
    /// </summary>
    public enum DualSpeedType
    {
        SPEED_2400,        
        SPEED_4800,
        SPEED_9600,
        SPEED_19200,
        SPEED_38400,
        SPEED_57600,
        SPEED_115200,
    }

    public static class DualSpeedTypeExtensions
    {
        public static int GetSpeed(this DualSpeedType e)
        {
            switch (e)
            {
                case DualSpeedType.SPEED_2400:
                    return 2400;
                case DualSpeedType.SPEED_4800:
                    return 4800;
                case DualSpeedType.SPEED_9600:
                    return 9600;
                case DualSpeedType.SPEED_19200:
                    return 19200;
                case DualSpeedType.SPEED_38400:
                    return 38400;
                case DualSpeedType.SPEED_57600:
                    return 57600;
                case DualSpeedType.SPEED_115200:
                default:
                    return 115200;
            }
        }

        public static DualSpeedType GetFromSpeed(this DualSpeedType e, int speed)
        {
            switch (speed)
            {
                case 2400:
                    return DualSpeedType.SPEED_2400;
                case 4800:
                    return DualSpeedType.SPEED_4800;
                case 9600:
                    return DualSpeedType.SPEED_9600;
                case 19200:
                    return DualSpeedType.SPEED_19200;
                case 38400:
                    return DualSpeedType.SPEED_38400;
                case 57600:
                    return DualSpeedType.SPEED_57600;
                case 115200:
                    return DualSpeedType.SPEED_115200;
                default:
                    return e;
            }
        }

    }

}