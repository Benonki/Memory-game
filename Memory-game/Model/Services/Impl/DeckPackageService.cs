using System.IO;
using System.IO.Compression;
using Memory_game.Model.Services;

namespace Memory_game.Model.Services.Impl
{
    public class DeckPackageService : IDeckPackageService
    {
        public byte[] CreateDeckZip(string deckName, string[] imagePaths)
        {
            using MemoryStream memoryStream = new MemoryStream();

            using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (string imagePath in imagePaths)
                {
                    if (!File.Exists(imagePath))
                        continue;

                    string entryName = Path.GetFileName(imagePath);

                    ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

                    using Stream entryStream = entry.Open();
                    using FileStream fileStream = File.OpenRead(imagePath);

                    fileStream.CopyTo(entryStream);
                }
            }

            return memoryStream.ToArray();
        }

        public bool DeckExistsAndMatches(string deckName, int expectedCardCount)
        {
            string deckPath = GetDeckPath(deckName);

            if (!Directory.Exists(deckPath))
                return false;

            int existingPngCount = Directory.GetFiles(deckPath, "*.png").Length;

            return existingPngCount >= expectedCardCount;
        }

        public void SaveDeckZip(string deckName, byte[] deckZipBytes, bool overwriteExisting = true)
        {
            if (deckZipBytes == null || deckZipBytes.Length == 0)
                return;

            string deckPath = GetDeckPath(deckName);

            if (Directory.Exists(deckPath) && overwriteExisting)
            {
                Directory.Delete(deckPath, recursive: true);
            }

            Directory.CreateDirectory(deckPath);

            using MemoryStream memoryStream = new MemoryStream(deckZipBytes);
            using ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Name))
                    continue;

                string destinationPath = Path.Combine(deckPath, entry.Name);

                entry.ExtractToFile(destinationPath, overwrite: true);
            }
        }

        private string GetDeckPath(string deckName)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MemoryGame",
                "Decks",
                deckName);
        }
    }
}