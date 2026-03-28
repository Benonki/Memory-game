using System.IO;

namespace Memory_game.Model.Services.Impl
{
    public class CardDeckServiceImpl : ICardDeckService
    {
        private readonly string _decksDirectory;
        private readonly string _defaultDeckDirectory;

        public CardDeckServiceImpl()
        {
            _decksDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MemoryGame", "Decks");
            _defaultDeckDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Cards", "Default");

            InitializeDecksDirectory();
        }

        private void InitializeDecksDirectory()
        {
            if (!Directory.Exists(_decksDirectory))
            {
                Directory.CreateDirectory(_decksDirectory);
            }

            string defaultDeckDirectory = Path.Combine(_decksDirectory, "Default");

            if (!Directory.Exists(defaultDeckDirectory))
            {
                Directory.CreateDirectory(defaultDeckDirectory);
                
                foreach (string file in Directory.GetFiles(_defaultDeckDirectory, "*.png"))
                {
                    File.Copy(file, Path.Combine(defaultDeckDirectory, Path.GetFileName(file)));
                }
            }
        }

        public IEnumerable<string> GetAllDecks()
        {
            return Directory.GetDirectories(_decksDirectory).Select(Path.GetFileName);
        }

        public string[] GetCardsFromDeck(string deckName)
        {
            string deckPath = Path.Combine(_decksDirectory, deckName);
            if (Directory.Exists(deckPath))
            {
                return Directory.GetFiles(deckPath, "*.png");
            }
            return Array.Empty<string>();
        }

        public void CreateDeck(string deckName, string[] imagePaths)
        {
            string deckPath = Path.Combine(_decksDirectory, deckName);
            if (!Directory.Exists(deckPath))
            {
                Directory.CreateDirectory(deckPath);
                foreach (string imagePath in imagePaths)
                {
                    string destPath = Path.Combine(deckPath, Path.GetFileName(imagePath));
                    File.Copy(imagePath, destPath, overwrite: true);
                }
            }
        }

        public void AddCardsToDeck(string deckName, string[] imagePaths)
        {
            string deckPath = Path.Combine(_decksDirectory, deckName);
            foreach (string file in imagePaths)
            {
                File.Copy(file, Path.Combine(deckPath, Path.GetFileName(file)), overwrite: true);
            }
        }

        public void DeleteDeck(string deckName)
        {
            string deckPath = Path.Combine(_decksDirectory, deckName);
            if (Directory.Exists(deckPath))
            {
                Directory.Delete(deckPath, recursive: true);
            }
        }

        public int GetCardCount(string deckName)
        {
            string deckPath = Path.Combine(_decksDirectory, deckName);
            if (Directory.Exists(deckPath))
            {
                return Directory.GetFiles(deckPath, "*.png").Length;
            }
            return 0;
        }
    }
}
