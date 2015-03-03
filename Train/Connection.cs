using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace Train
{


    class Connection : IDisposable
    {
        const int recBufferCount = 2048;    // 20 * 12 * 3 < 2048

        private string hostname;
        private int sendingPort;
        private int receivingPort;
        private TcpClient sendingSocket;
        private TcpClient receivingSocket;

        public Connection(int recPort, int sendPort, string host)
        {
            receivingPort = recPort;
            sendingPort = sendPort;
            hostname = host;

            receivingSocket = new TcpClient();
            sendingSocket = new TcpClient();
        }

        public async Task Connect()
        {
            try
            {
                await sendingSocket.ConnectAsync(hostname, sendingPort);
                await receivingSocket.ConnectAsync(hostname, receivingPort);
                Debug.WriteLine("Connected");
            }
            catch (Exception)
            {
                if (sendingSocket.Connected)
                    sendingSocket.Close();
                Debug.WriteLine("Chyba: nepodarilo se pripojit k serveru");
                throw;
            }

        }

        public async Task<byte[]> Receive()
        {
            byte[] recBuffer = new byte[recBufferCount];
            var stream = receivingSocket.GetStream();

            int ret = await stream.ReadAsync(recBuffer, 0, recBufferCount);

            // hack - TIME!
            var buffer = new byte[ret];
            Array.Copy(recBuffer, buffer, ret);

            return buffer;

        }

        public async Task Sending(byte[] toSent)
        {
            var stream = sendingSocket.GetStream();
            await stream.WriteAsync(toSent, 0, toSent.Count());
        }

        public void Dispose()
        {
            Debug.WriteLine("Closing sockets");
            if (sendingSocket.Connected)
                sendingSocket.Close();
            receivingSocket.Close();
        }
    }
}
