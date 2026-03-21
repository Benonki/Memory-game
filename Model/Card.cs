namespace Memory_game.Model
{
    public class Card
    {
        public string content { get; set; } = string.Empty;
        public bool isFaceUp { get; set; } = false;
        public bool isMatched { get; set; }
    }
}
