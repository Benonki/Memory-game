namespace Memory_game.Model.Services
{
    public interface ICardDeckService
    {
        IEnumerable<string> GetAllDecks();
        string[] GetCardsFromDeck(string deckName);
        void CreateDeck(string deckName, string[] imagePaths);
        void DeleteDeck(string deckName);
        int GetCardCount(string deckName);
        void AddCardsToDeck(string deckName, string[] imagePaths);
    }
}