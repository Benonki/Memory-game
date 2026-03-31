using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_game.Model.Services
{
    public interface IServerListener
    {
        event Action<string> ServerFound;
        public Task StartListeningAsync();
        public void StopListening();
    }
}
