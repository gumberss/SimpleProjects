using System;

namespace Naive_Bayes
{
    public class Probability
    {
        public Probability(String propName, String propValue, bool success)
        {
            PropertyName = propName;
            PropertyValue = propValue;
            Success = success;
        }

        public String PropertyName { get; set; }

        public String PropertyValue { get; set; }

        public bool Success { get; set; }

        public decimal ProbabilityValue { get; set; }

        public int Quantity { get; set; }

        public void Increase()
        {
            Quantity++;
        }

        public void CreateProbability(long totalElements, long successData)
        {
            if (Success)
                    ProbabilityValue = (decimal)Quantity / (totalElements - (totalElements - successData));
            else
                ProbabilityValue = (decimal)Quantity / (totalElements - successData);
        }
    }
}
