using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Naive_Bayes
{
    public class NaiveBayes
    {
        private List<String> _propertyNames;
        private PropertyInfo _classPropertyName;
        private List<PropertyInfo> _dataPropertyInfo;
        private Type _dataType;

        private List<Object> _trainingData;
        private List<Probability> _probabilities;

        private long _successCount;

        public NaiveBayes()
        {
            _trainingData = new List<Object>();
            _propertyNames = new List<String>();
            _dataPropertyInfo = new List<PropertyInfo>();

            _probabilities = new List<Probability>();
            _successCount = 0;
        }

        public void Add(Object newData)
        {
            if (newData == null)
                throw new ArgumentNullException("newData");

            if (!_trainingData.Any())
            {
                _trainingData.Add(newData);
                _dataType = newData.GetType();
                _dataPropertyInfo = _dataType.GetProperties().ToList();
                _propertyNames = _dataPropertyInfo.Select(i => i.Name).ToList();
                _classPropertyName = _dataPropertyInfo.FirstOrDefault(i => i.CustomAttributes.Any(j => j.AttributeType == typeof(NaiveBayesPropertyClassAttribute)));

                if (_classPropertyName == null)
                    throw new ArgumentException("É necessário definir a propriedade de decisão para o algoritmo colocando sobre alguma propriedade o atributo NaiveBayesPropertyClassAttribute");

                return;
            }

            if (!IsValidProperties(newData))
                throw new InvalidOperationException("Propriedades do novo objeto são diferente das propriedades dos objetos já adicionados");

            _trainingData.Add(newData);
        }

        public void Fit()
        {
            if (!_trainingData.Any())
                throw new InvalidOperationException("Não há dados para treinar o algoritmo");

            foreach (var currentTraining in _trainingData)
            {
                var success = (bool)_classPropertyName.GetValue(currentTraining);

                if (success) _successCount++;

                foreach (var propName in _propertyNames)
                {
                    var currentPropertyInfo = _dataPropertyInfo.First(i => i.Name == propName);

                    var propValue = currentPropertyInfo.GetValue(currentTraining)?.ToString();

                    var probability = _probabilities.FirstOrDefault(i =>
                        i.Success == success
                        && i.PropertyName == propName
                        && i.PropertyValue == propValue);

                    if (probability == null)
                    {
                        probability = new Probability(propName, propValue, success);

                        _probabilities.Add(probability);
                    }

                    probability.Increase();
                }
            }

            _probabilities.ForEach(i => i.CreateProbability(_trainingData.Count, _successCount));
        }

        public decimal Predict(Object dataToPredict, bool positiveResult = true)
        {
            if (!IsValidProperties(dataToPredict))
                throw new InvalidOperationException("Propriedades do novo objeto são diferente das propriedades dos objetos já adicionados");

            decimal finalProbability = 1;

            foreach (var propName in _propertyNames)
            {
                var value = _dataPropertyInfo
                                .First(i => i.Name == propName)
                                .GetValue(dataToPredict);

                var currentProbability = _probabilities.FirstOrDefault(i =>
                                                    i.PropertyName == propName
                                                 && i.PropertyValue.Equals(value?.ToString())
                                                 && i.Success == positiveResult);

                if (currentProbability == null) continue;

                finalProbability *= currentProbability.ProbabilityValue;
            }

            if (positiveResult)
                finalProbability *= ((decimal)_successCount / _trainingData.Count);
            else
                finalProbability *= ((decimal)(_trainingData.Count - _successCount) / _trainingData.Count);

            return finalProbability;
        }

        public bool PredictExactly(Object dateToPredict)
        {
            var positiveResult = Predict(dateToPredict);
            var negativeResult = Predict(dateToPredict, false);

            //caso igual??
            return positiveResult > negativeResult;
        }

        public List<Object> GetTrainingList()
        {
            return _trainingData;
        }

        public List<Probability> GetProbabilities()
        {
            return _probabilities;
        }

        private bool IsValidProperties(object newData)
        {
            var newPropertyNames = newData.GetType().GetProperties().Select(i => i.Name);

            var equalProperties = _propertyNames.All(newPropertyNames.Contains) && newPropertyNames.All(_propertyNames.Contains);

            return equalProperties;
        }
    }
}
