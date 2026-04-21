namespace Memory_game.Model.Services
{
    public interface IDeckPackageService
    {
        byte[] CreateDeckZip(string deckName, string[] imagePaths);
        bool DeckExistsAndMatches(string deckName, int expectedCardCount);
        void SaveDeckZip(string deckName, byte[] deckZipBytes, bool overwriteExisting = true);
    }
}