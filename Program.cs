using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    internal class TcpServer
    {
        private const int bufferSize = 32;

        static void Main(string[] args)
        {
            int port;
            int byteCount;
            int totalBytesEchoed = 0;
            byte[] receiveBuffer;
            TcpListener listener = null;
            TcpClient client = null;
            NetworkStream networkStream = null;

            if (args.Length > 1)
            {
                throw new ArgumentException("Port");
            }

            port = (args.Length == 1) ? int.Parse(args[0]) : 81;

            try
            {
                listener = new TcpListener(IPAddress.Any, port);

                listener.Start();

                Console.WriteLine("Listening...");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"{ex.ErrorCode}: {ex.Message}");

                Environment.Exit(ex.ErrorCode);
            }

            receiveBuffer = new byte[bufferSize];

            while(true)
            {
                try
                {
                    client = listener.AcceptTcpClient();

                    networkStream = client.GetStream();

                    while ((byteCount = networkStream.Read(receiveBuffer, 0, receiveBuffer.Length)) > 0)
                    {
                        networkStream.Write(receiveBuffer, 0, byteCount);

                        totalBytesEchoed += byteCount;
                    }

                    Console.WriteLine($"Received {totalBytesEchoed} bytes");
                    Console.WriteLine($"{Encoding.ASCII.GetString(receiveBuffer, 0, totalBytesEchoed)}\n");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                finally
                {
                    networkStream.Close();
                    client.Close();
                }
            }
        }
    }
}
