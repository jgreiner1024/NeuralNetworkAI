using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet
{
    public class BackPropagationTraining
    {

        public void Train(NeuralNetwork neuralNet, TrainingData[] trainingData, int maxEpochs, double learnRate, double momentum)
        {
            //weight gradients
            SimpleMatrix[] weightGradients = new SimpleMatrix[neuralNet.Weights.Length];
            for (int i = 0; i < weightGradients.Length; i++)
            { 
                int xLength = neuralNet.Weights[i].Values.GetLength(0);
                int yLength = neuralNet.Weights[i].Values.GetLength(1);
                weightGradients[i] = new SimpleMatrix(xLength, yLength, 0d);
            }

            SimpleMatrix[] weightDeltas = new SimpleMatrix[neuralNet.Weights.Length];
            for (int i = 0; i < weightGradients.Length; i++)
            {
                int xLength = neuralNet.Weights[i].Values.GetLength(0);
                int yLength = neuralNet.Weights[i].Values.GetLength(1);
                weightDeltas[i] = new SimpleMatrix(xLength, yLength, 0d);
            }

            //don't need gradients, signals or biases for input layer
            double[][] nodeLayerGradients = new double[neuralNet.NodeLayers.Length - 1][];
            double[][] nodeLayerSignals = new double[neuralNet.NodeLayers.Length - 1][];
            double[][] nodeLayerBiasDeltas = new double[neuralNet.NodeLayers.Length - 1][];
            for(int i = 0; i < nodeLayerGradients.Length; i++)
            {
                // add + 1 because we skip the input layer
                int len = neuralNet.NodeLayers[i + 1].Values.Length;
                nodeLayerGradients[i] = new double[len];
                nodeLayerSignals[i] = new double[len];
                nodeLayerBiasDeltas[i] = new double[len];
            }

            //randomize the order which we use the training data
            int[] trainingOrder = new int[trainingData.Length];
            for (int i = 0; i < trainingData.Length; ++i)
            {
                trainingOrder[i] = i;
            }

            Random rng = new Random();
            for (int i = 0; i < trainingData.Length; ++i)
            {
                int swapIndex = rng.Next(0, trainingData.Length);
                int swapValue = trainingOrder[swapIndex];
                trainingOrder[swapIndex] = trainingOrder[i];
                trainingOrder[i] = swapValue;
            }


            int errorInterval = maxEpochs / 100; // interval to check error
            for(int epoch = 0; epoch < maxEpochs; epoch++)
            {
                if (epoch % errorInterval == 0 && epoch < maxEpochs)
                {
                    double trainErr = Error(neuralNet, trainingData);
                    Console.WriteLine("epoch = " + epoch + "  error = " + trainErr.ToString("F4"));
                    //Console.ReadLine();
                }

                for (int i = 0; i < trainingData.Length; i++)
                {
                    int orderIndex = trainingOrder[i];

                    double[] result = neuralNet.FeedForward(trainingData[orderIndex].Input);

                    //calculate the output signal values
                    for(int index = 0; index < result.Length; index++)
                    //Parallel.For(0, result.Length, index => 
                    {
                        double val = (trainingData[orderIndex].Output[index] - result[index]) * ((1 - result[index]) * result[index]);
                        nodeLayerSignals[nodeLayerSignals.Length - 1][index] = val;
                        nodeLayerGradients[nodeLayerSignals.Length - 1][index] = val * 1d; //dummy value for start
                    }//);

                    //go backwards through the gradients and apply stuffs
                    for (int weightIndex = weightGradients.Length - 1; weightIndex >= 0 ; weightIndex--)
                    {
                        for(int xIndex = 0; xIndex < weightGradients[weightIndex].Values.GetLength(0); xIndex++)
                        //Parallel.For(0, weightGradients[weightIndex].Values.GetLength(0), xIndex =>
                        {
                            double nodeValue = neuralNet.NodeLayers[weightIndex].Values[xIndex];
                            double derivative = (1 + nodeValue) * (1 - nodeValue); // for tanh
                            double sum = 0.0; // need sums of output signals times hidden-to-output weights
                            for (int yIndex = 0; yIndex < weightGradients[weightIndex].Values.GetLength(1); yIndex++)
                            {
                                weightGradients[weightIndex].Values[xIndex, yIndex] = nodeLayerSignals[weightIndex][yIndex] * nodeValue;
                                sum += nodeLayerSignals[weightIndex][yIndex] * neuralNet.Weights[weightIndex].Values[xIndex, yIndex];
                            }

                            if(weightIndex - 1 >= 0)
                            {
                                double val = derivative * sum;
                                nodeLayerSignals[weightIndex - 1][xIndex] = val;
                                nodeLayerGradients[weightIndex - 1][xIndex] = val * 1d; // dummy 1.0 input
                            }
                        }//);
                    }

                    // time to go forward and update stuff
                    for (int weightIndex = 0; weightIndex < neuralNet.Weights.Length; weightIndex++)
                    {
                        for (int xIndex = 0; xIndex < neuralNet.Weights[weightIndex].Values.GetLength(0); xIndex++)
                        //Parallel.For(0, neuralNet.Weights[weightIndex].Values.GetLength(0), xIndex =>
                        {
                            for (int yIndex = 0; yIndex < neuralNet.Weights[weightIndex].Values.GetLength(1); yIndex++)
                            {
                                double delta = weightGradients[weightIndex].Values[xIndex,yIndex] * learnRate;

                                neuralNet.Weights[weightIndex].Values[xIndex, yIndex] += delta; // would be -= if (o-t)
                                neuralNet.Weights[weightIndex].Values[xIndex, yIndex] += weightDeltas[weightIndex].Values[xIndex, yIndex] * momentum;
                                weightDeltas[weightIndex].Values[xIndex, yIndex] = delta; // save for next time
                            }
                        }//);
                    }

                    for(int layerIndex = 1; layerIndex < neuralNet.NodeLayers.Length; layerIndex++)
                    {

                        for (int nodeIndex = 0; nodeIndex < neuralNet.NodeLayers[layerIndex].Biases.Length; ++nodeIndex)
                        //Parallel.For(0, neuralNet.NodeLayers[layerIndex].Biases.Length, nodeIndex =>  
                        {
                            
                            double delta = nodeLayerGradients[layerIndex - 1][nodeIndex] * learnRate;
                            neuralNet.NodeLayers[layerIndex].Biases[nodeIndex] += delta;
                            neuralNet.NodeLayers[layerIndex].Biases[nodeIndex] += nodeLayerBiasDeltas[layerIndex - 1][nodeIndex] * momentum;
                            nodeLayerBiasDeltas[layerIndex - 1][nodeIndex] = delta;
                        }//);
                    }

                }

            }
  
        }
        private double Error(NeuralNetwork neuralNet, TrainingData[] trainingData)
        {
            double totalErrorRating = 0d;
            for (int i = 0; i < trainingData.Length; i++)
            {
                double[] result = neuralNet.FeedForward(trainingData[i].Input);
                for(int r = 0; r < result.Length; r++)
                {
                    double errorRate = trainingData[i].Output[r] - result[r];
                    totalErrorRating += (errorRate * errorRate);

                }
            }

            return totalErrorRating / trainingData.Length;
        }

        public double Accuracy(NeuralNetwork neuralNet, TrainingData[] trainingData)
        {
            int numberCorrect = 0;
            int numberWrong = 0;
            for (int i = 0; i < trainingData.Length; i++)
            {
                double[] result = neuralNet.FeedForward(trainingData[i].Input);
                if(NeuralMath.MaxIndex(result) == NeuralMath.MaxIndex(trainingData[i].Output))
                {
                    numberCorrect++;
                }
                else
                {
                    numberWrong++;
                }
            }

            return (double)numberCorrect / (double)(numberCorrect + numberWrong);
        }
    }
}
