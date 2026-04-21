using Memory_game.Model.Services;
using Memory_game.Model.Services.Impl;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Memory_game
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IDeckPackageService DeckPackageService { get; } = new DeckPackageService();
        public static ILobbyService SharedLobbyService { get; } = new LobbyService(DeckPackageService);

    }

}
