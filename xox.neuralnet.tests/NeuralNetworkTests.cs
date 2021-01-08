using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xox.neuralnet.tests
{
    [TestClass]
    public class NeuralNetworkTests
    {
        

        



        [TestMethod]
        public void ItCanEvolveRockPaperScissors()
        {
            Random rng = new Random();

            int parentScore = 0;
            NeuralNetwork parentNetwork = new NeuralNetwork(3, 3, 3);

            //try 100 generations
            for(int gen = 0; gen < 10000; gen++)
            {
                //create 100 mutations
                NeuralNetwork[] children = new NeuralNetwork[100];
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = new NeuralNetwork(parentNetwork);
                    children[i].Mutate();
                }

                //we've achieved the best score
                if (parentScore == 3)
                {
                    Console.WriteLine($"Max Score found in generation {gen - 1}");
                    break;
                }

                //set up the score the children
                int highestScore = 0;
                NeuralNetwork potentialWinner = parentNetwork;
                for (int i = 0; i < children.Length; i++)
                {
                    int score = 0;

                    double[] result;

                    //rock
                    result = children[i].FeedForward(new double[] { 1d, 0d, 0d });
                    if (NeuralMath.MaxIndex(result) == 1) //check if our result is paper
                    {
                        score += 1;
                    }

                    //paper
                    result = children[i].FeedForward(new double[] { 0d, 1d, 0d });
                    if (NeuralMath.MaxIndex(result) == 2) //check if our result is scissors
                    {
                        score += 1;
                    }

                    //scissors
                    result = children[i].FeedForward(new double[] { 0d, 0d, 1d });
                    if (NeuralMath.MaxIndex(result) == 0) //check if our result is rock
                    {
                        score += 1;
                    }

                    if ((score > highestScore) || (score == highestScore && NeuralMath.RandomRange(rng, 0, 2) == 0))
                    {
                        highestScore = score;
                        potentialWinner = children[i];
                    }
                }

                if (highestScore > parentScore)
                {
                    Console.WriteLine($"New high score {highestScore} found in generation {gen}");
                    parentScore = highestScore;
                    parentNetwork = potentialWinner;
                }
            }

            Assert.AreEqual(3, parentScore);
        }
    }
}
