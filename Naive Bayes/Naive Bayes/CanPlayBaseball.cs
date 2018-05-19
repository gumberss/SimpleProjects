using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naive_Bayes
{
    public class CanPlayBaseball
    {
        public String Outlook { get; set; }

        public String Temperature { get; set; }

        public String Humidity { get; set; }

        public bool Windy { get; set; }

        [NaiveBayesPropertyClass]
        public bool CanPlay { get; set; }
    }
}
