namespace POSIDigitalPrinter.Enumerator
{
    internal static class QRCodeSizeExtensions
    {
        public static int GetSizeToDatec(this QRCodeSize t)
        {
            switch (t)
            {
                case QRCodeSize.SMALL:
                    return 2;
                case QRCodeSize.MEDIUM:
                    return 6;
                case QRCodeSize.LARGE:
                    return 10;
                default:
                    return 14;
            }
        }

        public static int GetSizeToNitere(this QRCodeSize t)
        {
            switch (t)
            {
                case QRCodeSize.SMALL:
                    return 2;
                case QRCodeSize.MEDIUM:
                    return 4;
                case QRCodeSize.LARGE:
                    return 6;
                default:
                    return 8;
            }
        }

        public static int GetSizeToCIS(this QRCodeSize t)
        {
            switch (t)
            {
                case QRCodeSize.SMALL:
                    return 10;
                case QRCodeSize.MEDIUM:
                    return 20;
                case QRCodeSize.LARGE:
                    return 30;
                default:
                    return 40;
            }
        }

        public static int GetSizeToRongta(this QRCodeSize t)
        {
            switch (t)
            {
                case QRCodeSize.SMALL:
                    return 1;
                case QRCodeSize.MEDIUM:
                    return 3;
                case QRCodeSize.LARGE:
                    return 6;
                default:
                    return 9;
            }
        }
    }

    public enum QRCodeSize
    {
        SMALL, 
        MEDIUM, 
        LARGE, 
        VERY_LARGE
    }
}