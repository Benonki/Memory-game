using Memory_game_server.Hubs;
using Memory_game_server.Services;
using Memory_game_server.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Memory_game.Model.Services.Impl
{
    public class ServerManagerImpl : IServerManager
    {
        private WebApplication? _app;

        public async Task StartServerAsync(int port)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();

            builder.Services.AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = 50 * 1024 * 1024;
            });

            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            _app = builder.Build();

            _app.MapHub<GameHub>("/gamehub");

            await _app.StartAsync();

        }

        public async Task StopServerAsync()
        {
            if (_app != null)
            {
                await _app.StopAsync();
            }
                
        }
    }
}
