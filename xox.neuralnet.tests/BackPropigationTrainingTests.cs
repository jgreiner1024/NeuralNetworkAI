using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet.tests
{
    [TestClass]
    public class BackPropigationTrainingTests
    {
        [TestMethod]
        public void ItCanTrainAIFromAnotherAI()
        {
            int inputNodeCount = 4;
            int hiddenNodeCount = 5;
            int outputNodeCount = 3;


            NeuralNetwork originalNeuralNet = new NeuralNetwork(inputNodeCount, hiddenNodeCount, outputNodeCount);
            TrainingData[] data = MakeAllData(originalNeuralNet, 1000, 1024);

            //split the data
            TrainingData[] trainData = new TrainingData[800];
            TrainingData[] testData = new TrainingData[200];
            
            for(int i = 0; i < trainData.Length; i++)
            {
                trainData[i] = data[i];
            }

            for (int i = 0; i < testData.Length; i++)
            {
                testData[i] = data[i + 800];
            }

            int maxEpochs = 1000;
            double learnRate = 0.05;
            double momentum = 0.01;
            NeuralNetwork trainedNeuralNet = new NeuralNetwork(inputNodeCount, hiddenNodeCount, outputNodeCount);
            BackPropagationTraining bpTraining = new BackPropagationTraining();
            DateTime startTime = DateTime.Now;
            bpTraining.Train(trainedNeuralNet, trainData, maxEpochs, learnRate, momentum);
            DateTime endTime = DateTime.Now;
            Console.WriteLine($"Training took {(endTime - startTime).TotalMilliseconds}");

            double trainAccuracy = bpTraining.Accuracy(trainedNeuralNet, trainData);
            double testAccuracy = bpTraining.Accuracy(trainedNeuralNet, testData);
            Console.WriteLine($"trained accuracy = {trainAccuracy}, testaccuracy = {testAccuracy}");
            Assert.IsTrue(trainAccuracy >= 0.99d);
            Assert.IsTrue(testAccuracy >= 0.99d);
        }

        [TestMethod]
        public void ItCanTrainAIRockPaperScissors() 
        {
            NeuralNetwork neuralNet = new NeuralNetwork(3, 3, 3);
            TrainingData[] trainData = new TrainingData[3];
            trainData[0] = new TrainingData(new double[] { 1d, 0d, 0d }, new double[] { 0d, 1d, 0d });
            trainData[1] = new TrainingData(new double[] { 0d, 1d, 0d }, new double[] { 0d, 0d, 1d });
            trainData[2] = new TrainingData(new double[] { 0d, 0d, 1d }, new double[] { 1d, 0d, 0d });

            int maxEpochs = 1000;
            double learnRate = 0.05;
            double momentum = 0.01;
            BackPropagationTraining bpTraining = new BackPropagationTraining();
            DateTime startTime = DateTime.Now;
            bpTraining.Train(neuralNet, trainData, maxEpochs, learnRate, momentum);
            DateTime endTime = DateTime.Now;
            Console.WriteLine($"Training took {(endTime - startTime).TotalMilliseconds}");

            double trainAccuracy = bpTraining.Accuracy(neuralNet, trainData);
            Console.WriteLine($"trained accuracy = {trainAccuracy}");
            Assert.IsTrue(trainAccuracy >= 0.99d);

            double[] result;
            //check feed in rock, see if the result is paper
            result = neuralNet.FeedForward(new double[] { 1d, 0d, 0d });
            Assert.AreEqual(1, NeuralMath.MaxIndex(result));

            //check feed in paper see if the result is scissors 
            result = neuralNet.FeedForward(new double[] { 0d, 1d, 0d });
            Assert.AreEqual(2, NeuralMath.MaxIndex(result));

            //check feed in scissors if the result is rock
            result = neuralNet.FeedForward(new double[] { 0d, 0d, 1d });
            Assert.AreEqual(0, NeuralMath.MaxIndex(result));

        }

        private TrainingData[] MakeAllData(NeuralNetwork neuralNet, int dataSize, int seed)
        {
            //randomize the neuralnet
            neuralNet.Randomize();
            Random rng = new Random(seed);
            TrainingData[] data = new TrainingData[dataSize];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new TrainingData(neuralNet.InputNodeCount, neuralNet.OutputNodeCount);
                Parallel.For(0, data[i].Input.Length, inputIndex => {
                    data[i].Input[inputIndex] = NeuralMath.RandomRange(rng, -10d, 10d);
                });

                double[] result = neuralNet.FeedForward(data[i].Input);
                Array.Copy(result, 0, data[i].Output, 0, result.Length);
            }

            return data;
        } 
    }
}
