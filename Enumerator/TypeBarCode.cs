using System;
using System.Collections.Generic;
using System.Linq;

namespace POSIDigitalPrinter.Enumerator
{
    internal static class TypeBarCodeExtensions
    {
        public static int GetCodeToBematech(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 0;
                case TypeBarCode.STANDARD_2_OF_5:
                    return 1;
                case TypeBarCode.EAN_13:
                    return 2;
                case TypeBarCode.EAN_8:
                    return 3;
                case TypeBarCode.CODE_39:
                    return 69;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 5;
                case TypeBarCode.CODABAR:
                    return 6;
                case TypeBarCode.MSI:
                    return 7;
                case TypeBarCode.CODE_11:
                    return 8;
                case TypeBarCode.CODE_93:
                    return 72;
                case TypeBarCode.CODE_128:
                    return 73;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static int GetCodeToDaruma(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.EAN_13:
                    return 1;
                case TypeBarCode.EAN_8:
                    return 2;
                case TypeBarCode.STANDARD_2_OF_5:
                    return 3;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 4;
                case TypeBarCode.CODE_128:
                    return 5;
                case TypeBarCode.CODE_39:
                    return 6;
                case TypeBarCode.CODE_93:
                    return 7;
                case TypeBarCode.UPC_A:
                    return 8;
                case TypeBarCode.CODABAR:
                    return 9;
                case TypeBarCode.MSI:
                    return 10;
                case TypeBarCode.CODE_11:
                    return 11;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static int GetCodeToElgin(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 0;
                case TypeBarCode.STANDARD_2_OF_5:
                    return 1;
                case TypeBarCode.EAN_13:
                    return 2;
                case TypeBarCode.EAN_8:
                    return 3;
                case TypeBarCode.CODE_39:
                    return 69;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 5;
                case TypeBarCode.CODABAR:
                    return 6;
                case TypeBarCode.MSI:
                    return 7;
                case TypeBarCode.CODE_11:
                    return 8;
                case TypeBarCode.CODE_93:
                    return 72;
                case TypeBarCode.CODE_128:
                    return 73;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static int GetCodeToEpson(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 0;
                case TypeBarCode.STANDARD_2_OF_5:
                    return 1;
                case TypeBarCode.EAN_13:
                    return 2;
                case TypeBarCode.EAN_8:
                    return 3;
                case TypeBarCode.CODE_39:
                    return 4;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 5;
                case TypeBarCode.CODABAR:
                    return 6;
                case TypeBarCode.MSI:
                    return 7;
                case TypeBarCode.CODE_11:
                    return 8;
                case TypeBarCode.CODE_93:
                    return 72;
                case TypeBarCode.CODE_128:
                    return 73;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static int GetCodeToBixolon(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 65;
                case TypeBarCode.EAN_13:
                    return 67;
                case TypeBarCode.EAN_8:
                    return 68;
                case TypeBarCode.CODE_39:
                    return 69;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 70;
                case TypeBarCode.CODABAR:
                    return 71;
                case TypeBarCode.CODE_93:
                    return 72;
                case TypeBarCode.CODE_128:
                    return 73;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static int GetCodeToDatec(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 65;
                case TypeBarCode.EAN_13:
                    return 67;
                case TypeBarCode.EAN_8:
                    return 68;
                case TypeBarCode.CODE_39:
                    return 69;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 70;
                case TypeBarCode.CODABAR:
                    return 71;
                case TypeBarCode.CODE_93:
                    return 72;
                case TypeBarCode.CODE_128:
                    return 75;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static int GetCodeToPRT(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 0;
                case TypeBarCode.EAN_8:
                    return 2;
                case TypeBarCode.EAN_13:
                    return 3;
                case TypeBarCode.CODE_39:
                    return 4;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 5;
                case TypeBarCode.CODABAR:
                    return 6;
                case TypeBarCode.CODE_93:
                    return 7;
                case TypeBarCode.CODE_128:
                    return 8;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static int GetCodeToNitere(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 65;
                case TypeBarCode.EAN_13:
                    return 67;
                case TypeBarCode.EAN_8:
                    return 68;                
                case TypeBarCode.CODE_39:
                    return 69;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 70;
                case TypeBarCode.CODABAR:
                    return 71;
                case TypeBarCode.CODE_93:
                    return 72;
                case TypeBarCode.CODE_128:
                    return 73;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }

        public static byte GetByteToFujitsu(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 65;
                case TypeBarCode.EAN_13:
                    return 67;
                case TypeBarCode.EAN_8:
                    return 68;
                case TypeBarCode.CODE_39:
                    return 69;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 70;
                case TypeBarCode.CODABAR:
                    return 71;
                case TypeBarCode.CODE_128:
                    return 73;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }


        public static int GetCodeToSweda(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.UPC_A:
                    return 0;
                case TypeBarCode.STANDARD_2_OF_5:
                    return 1;
                case TypeBarCode.EAN_13:
                    return 2;
                case TypeBarCode.EAN_8:
                    return 3;
                case TypeBarCode.CODE_39:
                    return 4;
                case TypeBarCode.INTERLEAVED_2_OF_5:
                    return 5;
                case TypeBarCode.CODABAR:
                    return 6;
                case TypeBarCode.MSI:
                    return 7;
                case TypeBarCode.CODE_11:
                    return 8;
                case TypeBarCode.CODE_93:
                    return 72;
                case TypeBarCode.CODE_128:
                    return 73;
                default:
                    throw new ArgumentException("Tipo nao suportado!");
            }
        }


        /// <summary>
        /// Quantidade de digitos para gerar o codigo de barras.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int GetLength(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.EAN_8:
                    return 7;
                case TypeBarCode.EAN_13:
                    return 12;
                case TypeBarCode.UPC_A:
                    return 11;
                case TypeBarCode.CODE_11:
                    return 16;
                case TypeBarCode.CODE_128:
                    return 22;
                default:
                    throw new InvalidOperationException($"Codigo de barras {t} não possui tamanho fixo.");
            }
        }

        /// <summary>
        /// Obtem lista de caracteres validos para o codigo de barras.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static char[] GetValidChars(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.EAN_8:
                case TypeBarCode.EAN_13:
                case TypeBarCode.UPC_A:
                case TypeBarCode.CODE_11:
                    return new List<char>()
                        .AddRange('0', '9')
                        .ToArray();
                case TypeBarCode.CODE_39:
                case TypeBarCode.CODE_93:
                    return new List<char>()
                        .AddRange('0', '9')
                        .AddRange('A', 'Z')
                        .AddRange('-', '.', '%', '/', '$', ' ', '+')
                        .ToArray();                    
                case TypeBarCode.CODABAR:
                    return new List<char>()
                        .AddRange('0', '9')
                        .AddRange('$', '-', ':', '/', '.', '+')
                        .ToArray();
                case TypeBarCode.CODE_128:
                    return new List<char>()
                        .AddRange(char.MinValue, char.MaxValue)
                        .ToArray();
                default:
                    throw new InvalidOperationException($"Não foi possivel obter tipo de caracteres para o codigo de barras {t}");

            }
        }

        /// <summary>
        /// Gera codigo de barras randomico.
        /// </summary>
        /// <returns></returns>
        public static string GetRandomBarcode(this TypeBarCode t)
        {
            switch (t)
            {
                case TypeBarCode.EAN_8:
                case TypeBarCode.EAN_13:
                case TypeBarCode.UPC_A:
                    int length      = t.GetLength();
                    char[] chars    = t.GetValidChars();
                    int minValue    = 0;
                    int maxValue    = chars.Length - 1;
                    char[] result   = new char[length];
                    Random r        = new Random();

                    for(int i = 0; i < length; i++)
                    {
                        int index = r.Next(minValue, maxValue);
                        result[i] = chars[index];
                    }

                    return new string(result);
                default:
                    throw new InvalidOperationException($"Tipo de codigo de barras {t} não pode gerar codigo de barras randomico!");
            }
        }

        private static IList<char> AddRange(this IList<char> list, char start, char end)
        {
            for (char c = start; c <= end; c++)
            {
                list.Add(c);
            }

            return list;
        }
        
        private static IList<char> AddRange(this IList<char> list, params char[] args)
        {
            foreach(char c in args)
            {
                list.Add(c);
            }

            return list;
        }        
    }

    public enum TypeBarCode
    {
        EAN_13, 
        EAN_8,
        STANDARD_2_OF_5,
        INTERLEAVED_2_OF_5,
        CODE_128,
        CODE_39,
        CODE_93,
        UPC_A,
        CODABAR,
        MSI,
        CODE_11
    }
}