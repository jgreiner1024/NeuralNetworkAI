using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xox.neuralnet
{
    public class NeuralNetwork
    {
        public int InputNodeCount { get; private set; }
        public int OutputNodeCount { get; private set; }

        public NodeLayer[] NodeLayers { get; private set; }
        public NodeWeightMatrix[] Weights { get; private set; }

        public NeuralNetwork(int inputNodeCount, int outputNodeCount, bool randomize = true) : 
            this(inputNodeCount, new int[] { (inputNodeCount + outputNodeCount)  / 2} , outputNodeCount, randomize)
        {

        }

        public NeuralNetwork(int inputNodeCount, int hiddenNodeCount, int outputNodeCount, bool randomize = true) :
            this(inputNodeCount, new int[] { hiddenNodeCount }, outputNodeCount, randomize)
        {

        }

        public NeuralNetwork(int inputNodeCount, int[] hiddenLayerNodeCount, int outputNodeCount, bool randomize = true)
        {
            this.InputNodeCount = inputNodeCount;
            this.OutputNodeCount = outputNodeCount;

            //set up our layers of nodes
            this.NodeLayers = new NodeLayer[hiddenLayerNodeCount.Length + 2];
            //input layer
            this.NodeLayers[0] = new NodeLayer(inputNodeCount, false);

            //hidden middle layers
            for(int i = 0; i < hiddenLayerNodeCount.Length; i++)
            {
                this.NodeLayers[i + 1] = new NodeLayer(hiddenLayerNodeCount[i]);
            }

            //output layer
            this.NodeLayers[this.NodeLayers.Length - 1] = new NodeLayer(outputNodeCount);

            //set up the weights
            this.Weights = new NodeWeightMatrix[this.NodeLayers.Length - 1];
            for (int i = 0; i < this.Weights.Length; i++)
            {
                this.Weights[i] = new NodeWeightMatrix(this.NodeLayers[i].Values.Length, this.NodeLayers[i + 1].Values.Length);
            }
            
            if(randomize == true)
            {
                Randomize();
            }
        }

        public NeuralNetwork(NeuralNetwork source)
        {
            this.InputNodeCount = source.NodeLayers[0].Values.Length;
            this.OutputNodeCount = source.NodeLayers[source.NodeLayers.Length - 1].Values.Length;

            //set up our layers of nodes
            this.NodeLayers = new NodeLayer[source.NodeLayers.Length];
            for (int i = 0; i < source.NodeLayers.Length; i++)
            {
                this.NodeLayers[i] = new NodeLayer(source.NodeLayers[i]);
            }

            //set up the weights
            this.Weights = new NodeWeightMatrix[source.Weights.Length];
            for (int i = 0; i < this.Weights.Length; i++)
            {
                this.Weights[i] = new NodeWeightMatrix(source.Weights[i]);
            }
        }

        public double[] FeedForward(double[] inputValues)
        {
            Array.Copy(inputValues, 0, this.NodeLayers[0].Values, 0, inputValues.Length);

            for (int layerIndex = 0; layerIndex < this.NodeLayers.Length - 1; layerIndex++)
            {
                double[] output = this.Weights[layerIndex].Calculate(this.NodeLayers[layerIndex].Values);
                //Parallel.For(0, output.Length, index => 
                for(int index = 0; index < output.Length; index++)
                {
                    this.NodeLayers[layerIndex + 1].Values[index] =
                        (layerIndex + 1) != (this.NodeLayers.Length - 1) ? //is this the output layer
                        NeuralMath.HyperTan(this.NodeLayers[layerIndex + 1].Biases[index] + output[index]) :
                        this.NodeLayers[layerIndex + 1].Biases[index] + output[index]; //no hyper tan for the output we'll softmax it
                }//);
            }

            //softmax and copy the results
            double[] result = NeuralMath.Softmax(this.NodeLayers[this.NodeLayers.Length - 1].Values);
            Array.Copy(result, 0, this.NodeLayers[this.NodeLayers.Length - 1].Values, 0, result.Length);

            return result;
        }

        public void Mutate()
        {
            Random random = new Random();
            foreach (var weight in this.Weights)
            {
                weight.Mutate(random);
            }
        }

        public void Randomize()
        {
            Random random = new Random();
            foreach (var weight in this.Weights)
            {
                weight.Randomize(random);
            }
        }

    }
}
