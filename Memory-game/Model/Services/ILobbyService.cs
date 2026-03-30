using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game.Model.Services
{
    public interface ILobbyService
    {
        public Task ConnectAsync(string serverAddress);
        public Task JoinGameAsync();
        public Task SendMessageAsync();
        public Task DisconnectAsync();

    }
}
