using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game.Model.Services.Impl
{
    public class ServerListener : IServerListener
    {
        public event Action<string>? ServerFound;
        private UdpClient? udpClient;
        private bool running;

        public async Task StartListeningAsync()
        {
            udpClient?.Close();
            udpClient?.Dispose();

            udpClient = new UdpClient(7788);
            running = true;

            while (running)
            {
                try
                {
                    var result = await udpClient.ReceiveAsync();
                    var message = Encoding.UTF8.GetString(result.Buffer);

                    if (message.StartsWith("MEMORY_GAME_SERVER"))
                    {
                        var port = message.Split(':')[1];
                        var serverIP = result.RemoteEndPoint.Address.ToString();

                        ServerFound?.Invoke($"{serverIP}:{port}");
                    }
                }catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                    break;
                }
               
            }
        }

        public void StopListening()
        {
            running = false;
            udpClient?.Close();
        }
    }
}
