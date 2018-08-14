using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace POSIDigitalPrinter.Utils
{
    class SocketClient : IDisposable
    {
        private StreamSocket socket;

        public delegate void Error(string message);
        public event Error OnError;

        public async void Connect(string hostname, int port)
        {
            socket = new StreamSocket();
            socket.Control.KeepAlive = false;
            HostName host = new HostName(hostname);
            await socket.ConnectAsync(host, port.ToString());
        }

        public async void sendData(string message)
        {
            DataWriter writer = new DataWriter(this.socket.OutputStream);

            writer.WriteUInt32(writer.MeasureString(message));
            writer.WriteString(message);

            try
            {
                await writer.StoreAsync();
                await writer.FlushAsync();
            }
            catch(Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public void Dispose()
        {
            this.socket.Dispose();
        }
    }
}
