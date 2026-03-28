using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game.Model.Services
{
    public interface ILobbyService
    {
        public Task ConnectAsync();
        public Task SendMessageAsync();
        public Task DisconnectAsync();

    }
}
