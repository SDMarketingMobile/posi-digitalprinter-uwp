using POSIDigitalPrinter.Utils;
using System;

namespace POSIDigitalPrinter.Enumerator
{
    /// <summary>
    /// Tipos de impressoras DUAL.
    /// </summary>
    public enum DualPrinterType
    {
        BEMATECH, BIXOLON, CIS, DARUMA, DATEC, ELGIN, EPSON, FUJITSU, NITERE, PRT, RONGTA, SWEDA, GENERIC
    }

    public static class DualPrinterTypeExtensions
    {
        public static int GetCodigo(this DualPrinterType t)
        {
            switch (t)
            {
                case DualPrinterType.BEMATECH:
                    return 0;
                case DualPrinterType.BIXOLON:
                    return 1;
                case DualPrinterType.CIS:
                    return 2;
                case DualPrinterType.DARUMA:
                    return 3;
                case DualPrinterType.DATEC:
                    return 4;
                case DualPrinterType.ELGIN:
                    return 5;
                case DualPrinterType.EPSON:
                    return 6;
                case DualPrinterType.FUJITSU:
                    return 7;
                case DualPrinterType.NITERE:
                    return 8;
                case DualPrinterType.PRT:
                    return 9;
                case DualPrinterType.RONGTA:
                    return 10;
                case DualPrinterType.SWEDA:
                    return 11;
                default:
                    return 12;
            }
        }

        public static string GetName(this DualPrinterType t)
        {
            switch (t)
            {
                case DualPrinterType.BEMATECH:
                    return "Bematech";
                case DualPrinterType.BIXOLON:
                    return "Bixolon";
                case DualPrinterType.CIS:
                    return "Cis";
                case DualPrinterType.DARUMA:
                    return "Daruma";
                case DualPrinterType.DATEC:
                    return "Datec";
                case DualPrinterType.ELGIN:
                    return "Elgin";
                case DualPrinterType.EPSON:
                    return "Epson";
                case DualPrinterType.FUJITSU:
                    return "Fujitsu";
                case DualPrinterType.NITERE:
                    return "Nitere";
                case DualPrinterType.PRT:
                    return "Prt";
                case DualPrinterType.RONGTA:
                    return "Rongta";
                case DualPrinterType.SWEDA:
                    return "Sweda";
                case DualPrinterType.GENERIC:
                    return "Generic";
                default:
                    throw new ArgumentException("Parâmetro inválido");
            }
        }

        public static DualPrinterType FromCodigo(this DualPrinterType e, int code)
        {
            foreach (DualPrinterType aux in Enum.GetValues(typeof(DualPrinterType)))
            {
                if (aux.GetCodigo().Equals(code))
                {
                    return aux;
                }
            }
            return e;
        }

        public static T GetFromName<T>(this T e, string name) where T : struct
        {
            if (!String.IsNullOrEmpty(name))
            {
                foreach (T type in EnumUtil.GetValues<T>())
                {
                    if (type.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return type;
                    }
                }
            }

            return e;
        }

        /// <summary>
        /// Informa se tipo de impressora aceita comunicacao serial.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsSerialCommunication(this DualPrinterType e)
        {
            switch (e)
            {
                case DualPrinterType.BEMATECH:
                case DualPrinterType.BIXOLON:
                case DualPrinterType.CIS:
                case DualPrinterType.DARUMA:
                case DualPrinterType.ELGIN:
                case DualPrinterType.EPSON:
                case DualPrinterType.FUJITSU:
                case DualPrinterType.NITERE:
                case DualPrinterType.GENERIC:
                case DualPrinterType.DATEC:
                case DualPrinterType.PRT:
                case DualPrinterType.RONGTA:
                case DualPrinterType.SWEDA:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Informa se tipo de impressoa aceita comunicacao USB.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsUSBCommunication(this DualPrinterType e)
        {
            switch (e)
            {
                case DualPrinterType.CIS:
                case DualPrinterType.EPSON:
                case DualPrinterType.NITERE:
                case DualPrinterType.FUJITSU:
                case DualPrinterType.BEMATECH:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Informa se tipo de impressora aceita comunicação TCP/IP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsIPCommunication(this DualPrinterType e)
        {
            return false;
        }

        /// <summary>
        /// Verifica se tipo de impressora aceita tipo de conexão.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="conType"></param>
        /// <returns></returns>
        public static bool AcceptConnectionType(this DualPrinterType e, DualConnectionType conType)
        {
            switch (conType)
            {
                case DualConnectionType.SERIAL:
                case DualConnectionType.TCP_IP:
                    return e.IsSerialCommunication();
                default:
                    return e.IsUSBCommunication();
            }
        }
    }
}