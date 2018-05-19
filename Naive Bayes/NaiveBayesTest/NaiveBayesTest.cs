using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Naive_Bayes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayesTest
{
    [TestClass]
    public class NaiveBayesTest
    {
        private NaiveBayes _naiveBayes;

        [TestInitialize]
        public void Initialize()
        {
            _naiveBayes = new NaiveBayes();
        }

        #region add

        [TestMethod]
        public void Deveria_lancar_excecao_quando_tentar_adicionar_objeto_nulo_na_lista_de_treinamento()
        {

            Action action = () => _naiveBayes.Add(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [TestMethod]
        public void Deveria_adicionar_na_lista_de_treino_quando_for_o_primeiro_elemento()
        {
            _naiveBayes.Add(new PlayBasketball());

            _naiveBayes.GetTrainingList().Should().HaveCount(1);
        }

        [TestMethod]
        public void Deveria_lancar_excecao_quando_objeto_adicionado_na_lista_nao_possuir_propriedade_de_validacao()
        {
            Action action = () => _naiveBayes.Add(new NaiveBayesTest());

            action.Should().ThrowExactly<ArgumentException>()
                .And.Message.Should().Be("É necessário definir a propriedade de decisão para o algoritmo colocando sobre alguma propriedade o atributo NaiveBayesPropertyClassAttribute");
        }

        [TestMethod]
        public void Deveria_lancar_excecao_quando_tentar_adicionar_objeto_diferente_na_lista_de_treinamento()
        {
            _naiveBayes.Add(new PlayBasketball());

            Action action = () => _naiveBayes.Add(new object());

            action.Should().ThrowExactly<InvalidOperationException>()
                .And.Message.Should().Be("Propriedades do novo objeto são diferente das propriedades dos objetos já adicionados");
        }

        [TestMethod]
        public void Nao_deveria_lancar_excecao_quando_tentar_adicionar_novo_objeto_igual_ao_ja_existente()
        {
            _naiveBayes.Add(new PlayBasketball());
            _naiveBayes.Add(new PlayBasketball());

            _naiveBayes.GetTrainingList().Should().HaveCount(2);
        }

        #endregion add

        #region fit

        [TestMethod]
        public void Deveria_retornar_excecao_quando_treinar_sem_dados()
        {
            Action action = () => _naiveBayes.Fit();

            action.Should().ThrowExactly<InvalidOperationException>()
                .And.Message.Should().Be("Não há dados para treinar o algoritmo");
        }

        [TestMethod]
        public void Deveria_criar_probabilidade_com_cem_porcento_de_chance_quando_possuir_apenas_um_dado_no_treinamento()
        {
            var play = new PlayBasketball
            {
                HasBall = true,
                NumberOfPlayers = 4,
                Rain = false,
                Windy = false,
                CanPlay = true
            };

            _naiveBayes.Add(play);

            _naiveBayes.Fit();

            var correctValue = _naiveBayes.GetProbabilities().All(i => i.ProbabilityValue == 1);

            correctValue.Should().BeTrue();
        }

        [TestMethod]
        public void Deveria_criar_probabilidade_com_cinquenta_porcento_de_chance_quando_possuir_apenas_dois_dados_completamente_distintos_no_treinamento()
        {
            var play = new PlayBasketball
            {
                HasBall = true,
                NumberOfPlayers = 4,
                Rain = false,
                Windy = false,
                CanPlay = true
            };

            var play2 = new PlayBasketball
            {
                HasBall = false,
                NumberOfPlayers = 2,
                Rain = true,
                Windy = true,
                CanPlay = false
            };

            _naiveBayes.Add(play);
            _naiveBayes.Add(play2);

            _naiveBayes.Fit();

            var correctValue = _naiveBayes.GetProbabilities().All(i => i.ProbabilityValue == 1);

            correctValue.Should().BeTrue();
        }

        #endregion fit

        #region predict

        [TestMethod]
        public void Deveria_prever_com_cem_porcento_de_probabildade_quando_dado_fornecido_for_igual_ao_dado_treinado()
        {
            var play = new PlayBasketball
            {
                HasBall = true,
                NumberOfPlayers = 4,
                Rain = false,
                Windy = false,
                CanPlay = true
            };

            _naiveBayes.Add(play);

            _naiveBayes.Fit();

            decimal probability = _naiveBayes.Predict(play);

            probability.Should().Be(1);
        }

        [TestMethod]
        public void Deveria_calcular_probabilidade_com_base_nos_dados_fornecidos()
        {
            var play = new PlayBasketball
            {
                HasBall = true,
                NumberOfPlayers = 4,
                Rain = false,
                Windy = false,
                CanPlay = true
            };

            var play2 = new PlayBasketball
            {
                HasBall = false,
                NumberOfPlayers = 2,
                Rain = true,
                Windy = true,
                CanPlay = true
            };

            _naiveBayes.Add(play);
            _naiveBayes.Add(play2);

            _naiveBayes.Fit();

            decimal probabilityFalse = _naiveBayes.Predict(play,false);

            probabilityFalse.Should().Be(0, because: "Não há probabilidade de não jogar");
        }

        #endregion predict


        #region Predict exactly

        [TestMethod]
        public void Deveria_prever_que_ira_jogar_basquete_quando_dados_treinados_avaiarem_como_melhor_opcao_com_base_em_um_dado()
        {
            var play = new PlayBasketball
            {
                HasBall = true,
                NumberOfPlayers = 4,
                Rain = false,
                Windy = false,
                CanPlay = true
            };

            _naiveBayes.Add(play);

            _naiveBayes.Fit();

            bool canPlay = _naiveBayes.PredictExactly(play);

            canPlay.Should().Be(true);
        }

        [TestMethod]
        public void Deveria_prever_que_e_possivel_jogar()
        {
            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "sunny",
                Temperature = "hot",
                Humidity = "high",
                Windy = false,
                CanPlay = false
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "sunny",
                Temperature = "hot",
                Humidity = "high",
                Windy = true,
                CanPlay = false
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "overcast",
                Temperature = "hot",
                Humidity = "high",
                Windy = false,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "rainy",
                Temperature = "mild",
                Humidity = "high",
                Windy = false,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "rainy",
                Temperature = "cool",
                Humidity = "normal",
                Windy = false,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "rainy",
                Temperature = "cool",
                Humidity = "normal",
                Windy = true,
                CanPlay = false
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "overcast",
                Temperature = "cool",
                Humidity = "normal",
                Windy = true,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "sunny",
                Temperature = "mild",
                Humidity = "high",
                Windy = false,
                CanPlay = false
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "sunny",
                Temperature = "cool",
                Humidity = "normal",
                Windy = false,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "rainy",
                Temperature = "mild",
                Humidity = "normal",
                Windy = false,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "sunny",
                Temperature = "mild",
                Humidity = "normal",
                Windy = true,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "overcast",
                Temperature = "mild",
                Humidity = "high",
                Windy = true,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "overcast",
                Temperature = "hot",
                Humidity = "normal",
                Windy = false,
                CanPlay = true
            });

            _naiveBayes.Add(new CanPlayBaseball
            {
                Outlook = "rainy",
                Temperature = "mild",
                Humidity = "high",
                Windy = true,
                CanPlay = false
            }); 

            _naiveBayes.Fit();

            bool canPlay = _naiveBayes.PredictExactly(new CanPlayBaseball
            {
                Outlook = "sunny",
                Temperature = "hot",
                Humidity = "high",
                Windy = false,
                CanPlay = false
            });

            canPlay.Should().Be(false);
        }
        #endregion Predict exactly


        #region random

        [TestMethod]
        public void Random_values()
        {
            Stopwatch s = new Stopwatch();

            s.Start();

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

            s.Stop();

            s.ElapsedMilliseconds.Should().BeLessThan(8500);
        }

        #endregion random
    }
}

