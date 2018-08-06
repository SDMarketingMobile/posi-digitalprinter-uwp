using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;

namespace POSIDigitalPrinter.Utils
{
    class SocketServer
    {

        private readonly int _port;
        public int Port { get { return _port; } }

        private StreamSocketListener listener;
        private DataWriter _writer;

        public delegate void DataReceived(string data);
        public event DataReceived OnDataReceived;

        public delegate void Error(string message);
        public event Error OnError;

        public SocketServer(int port)
        {
            this._port = port;
        }

        public async void Start()
        {
            try
            {
                // Fecha a conexão com a porta que está escutando atualmente...
                if(listener != null)
                {
                    await listener.CancelIOAsync();
                    listener.Dispose();
                    listener = null;
                }

                // Cria uma nova instância do listener
                listener = new StreamSocketListener();

                // Adiciona o evento de conexão recebida ao método Listener_ConnectionReceived...
                listener.ConnectionReceived += Listener_ConnectionReceived;

                // Espera fazer o bind da porta...
                await listener.BindServiceNameAsync(Port.ToString());
            }
            catch (Exception e)
            {
                if (OnError != null)
                    OnError(e.Message);
            }
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender, 
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StringBuilder strBuidler;
            var reader = new DataReader(args.Socket.InputStream);
            _writer = new DataWriter(args.Socket.OutputStream);

            try
            {
                while(true)
                {
                    string DataReceived;

                    strBuidler = new StringBuilder();
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    reader.ByteOrder = ByteOrder.LittleEndian;
                    await reader.LoadAsync(4096);
                    while (reader.UnconsumedBufferLength > 0)
                    {
                        strBuidler.Append(reader.ReadString(reader.UnconsumedBufferLength));
                    }
                    reader.DetachStream();
                    DataReceived = strBuidler.ToString();

                    if (OnDataReceived != null)
                    {
                        // Dispara o evento de dado recebido...
                        OnDataReceived(DataReceived);
                    }
                }
            }
            catch (Exception e)
            {
                if (OnError != null)
                    OnError(e.Message);
            }
        }

        public async void Send(string message)
        {
            if(_writer != null)
            {
                // Envia o tamanho da string...
                _writer.WriteUInt32(_writer.MeasureString(message));

                // Envia a string em si...
                _writer.WriteString(message);

                try
                {
                    //Faz o envio da mensagem...
                    await _writer.StoreAsync();

                    await _writer.FlushAsync();
                }
                catch(Exception e)
                {
                    if (OnError != null)
                        OnError(e.Message);
                }
            }
        }
    }
}
