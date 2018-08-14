using System;

namespace POSIDigitalPrinter.Enumerator
{
    public enum PrintDeviceType
    {
        DUAL,
        DIGITAL_PRINTER
    }

    public static class PrintDeviceTypeExtension
    {
        public static int GetCodigo(this PrintDeviceType t)
        {
            switch (t)
            {
                case PrintDeviceType.DUAL:
                    return 0;
                default:
                    return 1;
            }
        }

        public static string GetName(this PrintDeviceType t)
        {
            switch (t)
            {
                case PrintDeviceType.DUAL:
                    return "Dual";
                case PrintDeviceType.DIGITAL_PRINTER:
                    return "Digital Printer";
                default:
                    throw new ArgumentException("Parâmetro inválido");
            }
        }

        public static string GetKitchenDescription(this PrintDeviceType t)
        {
            switch (t)
            {
                case PrintDeviceType.DIGITAL_PRINTER:
                    return "Digital Printer";
                case PrintDeviceType.DUAL:
                default:
                    return "Impressora";
            }
        }

        public static PrintDeviceType FromCodigo(this PrintDeviceType e, int code)
        {
            foreach (PrintDeviceType aux in System.Enum.GetValues(typeof(PrintDeviceType)))
            {
                if (aux.GetCodigo().Equals(code))
                {
                    return aux;
                }
            }
            return e;
        }
    }
}
