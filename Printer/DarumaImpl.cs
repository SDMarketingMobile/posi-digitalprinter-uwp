using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using POSIDigitalPrinter.Enumerator;
using System.Threading;

namespace POSIDigitalPrinter.Printer
{
    class DarumaImpl : IDisposable
    {
        private const int MAX_BUFFER_SIZE = 1024 * 3;

        private SerialDevice DeviceConnection;
        private MemoryStream Buffer;

        public DarumaImpl(SerialDevice deviceConnection)
        {
            this.DeviceConnection = deviceConnection;
            this.Buffer = new MemoryStream();
        }

        public void Dispose()
        {
            this.Flush();
            this.DeviceConnection.Dispose();
        }

        private void ClearBuffer()
        {
            this.Buffer.Close();
            this.Buffer = new MemoryStream();
        }

        public void Print(byte[] bytes)
        {
            this.Buffer.Write(bytes, 0, bytes.Length);
        }

        public void Print(string message)
        {
            byte[] bytes = Encoding.Default.GetBytes(message);
            this.Print(bytes);
        }

        private async void Flush()
        {
            try
            {
                int read;
                int length = MAX_BUFFER_SIZE;
                byte[] data = new byte[length];

                new StreamWriter(this.Buffer).Flush();
                this.Buffer.Position = 0;

                do
                {
                    read = this.Buffer.Read(data, 0, length);
                    var buffer = data.Take(read).ToArray().AsBuffer();
                    await this.DeviceConnection.OutputStream.WriteAsync(buffer);
                }
                while (read == length);
            }
            finally
            {
                this.ClearBuffer();
            }
        }

        public void Forward(int qtdLinesForward)
        {
            for(var i = 0; i < qtdLinesForward; i++)
            {
                this.Print(new byte[] { AsciiTable.EM.GetAsciiByte() });
            }
        }

        public void Beep()
        {
            this.Print(new byte[] { AsciiTable.BEL.GetAsciiByte() });
        }

        public void FontStyle(Style s)
        {
            switch(s)
            {
                case Style.PLAIN:
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x46 }); // sem negrito
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x2d, AsciiTable.NUL.GetAsciiByte() }); // sem sublinhado
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x34, AsciiTable.NUL.GetAsciiByte() }); // sem itálico
                    break;
                case Style.BOLD:
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x45 }); // sem negrito
                    break;
                case Style.UNDERLINE:
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x2d, AsciiTable.SOH.GetAsciiByte() }); // com sublinhado
                    break;
                case Style.ITALIC:
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x34, AsciiTable.SOH.GetAsciiByte() }); // com itálico
                    break;
            }
        }

        public void Font(Fonts f)
        {
            switch(f)
            {
                case Fonts.FONT_EXPANDED: // double wide expanded
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x21, 0x10 });
                    break;
                case Fonts.FONT_LARGE: // double tall expanded
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x21, 0x01 });
                    break;
                case Fonts.FONT_NORMAL: // character fonts: Font-A
                    this.Print(new byte[] { AsciiTable.DC2.GetAsciiByte() }); // cancela modo condensado
                    this.Print(new byte[] { AsciiTable.ESC.GetAsciiByte(), 0x21, 0x00 });
                    break;
                case Fonts.FONT_CONDENSED:
                    this.Print(new byte[] { AsciiTable.SI.GetAsciiByte() }); // seleciona modo condensado
                    break;
            }
        }
    }
}
