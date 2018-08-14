using System;

namespace POSIDigitalPrinter.Enumerator
{
    /// <summary>
    /// Tipos de conexões.
    /// </summary>
    public enum DualConnectionType
    {
        SERIAL, USB, TCP_IP
    }

    public static class DualConnectionTypeExtension
    {
        public static int GetCodigo(this DualConnectionType t)
        {
            switch (t)
            {
                case DualConnectionType.SERIAL:
                    return 0;
                case DualConnectionType.USB:
                    return 1;
                default:
                    return 2;
            }
        }

        public static string GetName(this DualConnectionType t)
        {
            switch (t)
            {
                case DualConnectionType.SERIAL:
                    return "Serial";
                case DualConnectionType.TCP_IP:
                    return "TCP/IP";
                case DualConnectionType.USB:
                    return "USB";
                default:
                    throw new ArgumentException("Parâmetro inválido");
            }
        }

        public static DualConnectionType FromCodigo(this DualConnectionType e, int code)
        {
            foreach (DualConnectionType aux in System.Enum.GetValues(typeof(DualConnectionType)))
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