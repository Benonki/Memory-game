namespace Memory_game.Model
{
    public class Card
    {
        public int pairId { get; set; }
        public string imagePath { get; set; } = string.Empty;
        public bool isFaceUp { get; set; } = false;
        public bool isMatched { get; set; }
    }
}
