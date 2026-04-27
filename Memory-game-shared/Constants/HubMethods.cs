namespace Memory_game_shared.Constants
{
    public static class HubMethods
    {
        public const string SendMessage = "SendMessage";
        public const string ReceiveMessage = "ReceiveMessage";
        public const string GameStarted = "GameStarted";
        public const string JoinGame = "JoinGame";
        public const string PlayerDisconnected = "PlayerDisconnected";
        public const string CreateNewGame = "CreateNewGame";
        public const string GameOver = "GameOver";
        public const string DeckPackage = "DeckPackage";
        public const string WaitingForPlayers = "WaitingForPlayers";

        // Game actions
        public const string FlipCard = "FlipCard";
        public const string MatchFound = "MatchFound";
        public const string MatchFailed = "MatchFailed";
        public const string ChangeTurn = "ChangeTurn";
        public const string TurnTimeout = "TurnTimeout";
    }
}
