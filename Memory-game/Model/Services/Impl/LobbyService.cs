using Memory_game_shared.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;

namespace Memory_game.Model.Services.Impl
{
    public class LobbyService : ILobbyService
    {

        HubConnection connection;

        public async Task ConnectAsync()
        {
            Debug.WriteLine("Trying to connect");
            try
            {
                connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/gamehub")
                .Build();

                connection.On<string>(HubMethods.ReceiveMessage, (message) =>
                {
                    Debug.WriteLine(message);
                });

                await connection.StartAsync();

                Debug.WriteLine(connection.State);
            }catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            

        }

        public async Task DisconnectAsync()
        {
            await connection.StopAsync();
        }

        public async Task SendMessageAsync()
        {
            await connection.InvokeAsync(HubMethods.SendMessage, "Message from client");
        }
    }
}
