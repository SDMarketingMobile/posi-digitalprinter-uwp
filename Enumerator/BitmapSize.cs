namespace POSIDigitalPrinter.Enumerator
{
    public enum BitmapSize
    {
        VERY_LARGE, LARGE, MEDIUM, SMALL
    }

    public static class BitmapSizeExtensions
    {
        public static int GetInt(this BitmapSize b)
        {
            switch (b)
            {
                case BitmapSize.VERY_LARGE:
                    return 550;
                case BitmapSize.LARGE:
                    return 430;
                case BitmapSize.MEDIUM:
                    return 310;
                default:
                    return 190;
            }
        }
    }
}