

using Memory_game_server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();


app.MapHub<GameHub>("/gamehub");
app.MapGet("/", () => "Server started");

app.Run();
