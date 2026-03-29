namespace Memory_game_server.Services
{
    public interface IBroadcastService
    {
        public Task StartBroadcastingAsync(int port);
        public void StopBroadcasting();
    }
}
