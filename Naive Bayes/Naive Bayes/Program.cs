using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naive_Bayes
{
    class Program
    {
        static void Main(string[] args)
        {
            NaiveBayes _naiveBayes = new NaiveBayes();

            Random r = new Random();

            for (int i = 0; i < 1000000; i++)
            {
                var play = new PlayBasketball
                {
                    HasBall = r.Next(1, 10) < 5 ? true : false,
                    NumberOfPlayers = r.Next(1, 10),
                    Rain = r.Next(1, 10) < 5 ? true : false,
                    Windy = r.Next(1, 10) < 5 ? true : false,
                    CanPlay = r.Next(1, 10) < 5 ? true : false
                };

                _naiveBayes.Add(play);
            }

            _naiveBayes.Fit();

            bool canPlay = _naiveBayes.PredictExactly(new PlayBasketball
            {
                HasBall = false,
                NumberOfPlayers = 2,
                Rain = true,
                Windy = true,
                CanPlay = false
            });
        }
    }
}
