

using Memory_game_server.Hubs;
using Memory_game_server.Services;
using Memory_game_server.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();


app.MapHub<GameHub>("/gamehub");
app.MapGet("/", () => "Server started");

IBroadcastService broadcastService = new BroadcastServiceImpl();
_ = broadcastService.StartBroadcastingAsync(5000);

app.Run();
