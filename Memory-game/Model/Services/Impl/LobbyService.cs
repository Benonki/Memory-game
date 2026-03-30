using Memory_game_shared.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;

namespace Memory_game.Model.Services.Impl
{
    public class LobbyService : ILobbyService
    {

        HubConnection? connection;

        public async Task JoinGame(string serverAddress)
        {
            Debug.WriteLine("Trying to connect");
            try
            {
                connection = new HubConnectionBuilder()
                .WithUrl($"http://{serverAddress}/gamehub")
                .Build();

                connection.On<string>(HubMethods.GameStarted, (message) =>
                {
                    Debug.WriteLine($"Joined game gmae status: {message}");
                });

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
            if(connection != null)
                await connection.StopAsync();
        }

        public async Task SendMessageAsync()
        {
            if(connection != null )
                await connection.InvokeAsync(HubMethods.SendMessage, "Message from client");
        }
    }
}
