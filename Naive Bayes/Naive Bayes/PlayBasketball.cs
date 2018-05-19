namespace Naive_Bayes
{
    public class PlayBasketball
    {
        public bool Rain { get; set; }

        public bool Windy { get; set; }

        public int NumberOfPlayers { get; set; }

        public bool HasBall { get; set; }

        [NaiveBayesPropertyClass]
        public bool CanPlay { get; set; }
    }
}
