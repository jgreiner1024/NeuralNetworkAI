using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet
{
    public class NodeWeightMatrix : SimpleMatrix
    {
        private int inputNodeCount;
        private int outputNodeCount;

        public NodeWeightMatrix(int inputCount, int outputCount) : 
            base(inputCount, outputCount, null)
        {
            inputNodeCount = inputCount;
            outputNodeCount = outputCount;
        }

        public NodeWeightMatrix(NodeWeightMatrix source) :
            this(source.inputNodeCount, source.outputNodeCount)
        {
            for(int inputIndex = 0; inputIndex < inputNodeCount; inputIndex++)
            {
                for (int outputIndex = 0; outputIndex < outputNodeCount; outputIndex++)
                {
                    this.Values[inputIndex, outputIndex] = source.Values[inputIndex, outputIndex];
                }
            }
        }

        public double[] Calculate(double[] input)
        {
            double[] output = new double[outputNodeCount];

            Parallel.For(0, outputNodeCount, 
                outputIndex => {
                    output[outputIndex] = 0;
                    for (int inputIndex = 0; inputIndex < inputNodeCount; inputIndex++)
                    {
                        output[outputIndex] += input[inputIndex] * this.Values[inputIndex, outputIndex];
                    }
                });

            return output;
        }

        public void Mutate(Random random)
        {
            Parallel.For(0, inputNodeCount, inputIndex => {
                for (int outputIndex = 0; outputIndex < outputNodeCount; outputIndex++)
                {
                    //mutate chance value 
                    int randomNumber = random.Next(0, 100);
                    if (randomNumber <= 2)
                    {
                        //flip sign of weight
                        this.Values[inputIndex, outputIndex] *= -1d;
                    }
                    else if (randomNumber <= 4)
                    {
                        //pick random weight between -1 and 1
                        this.Values[inputIndex, outputIndex] = NeuralMath.RandomRange(random , - 0.5d, 0.5d);
                    }
                    else if (randomNumber <= 6)
                    {
                        //randomly increase by 0% to 100%
                        this.Values[inputIndex, outputIndex] *= NeuralMath.RandomRange(random, 0d, 1d) + 1d;
                    }
                    else if (randomNumber <= 8)
                    {
                        //randomly decrease by 0% to 100%
                        this.Values[inputIndex, outputIndex] *= NeuralMath.RandomRange(random, 0d, 1d);
                    }
                }
            });
        }

        public void Randomize(Random random)
        {
            Parallel.For(0, inputNodeCount, inputIndex => {
                for (int outputIndex = 0; outputIndex < outputNodeCount; outputIndex++)
                {
                    this.Values[inputIndex, outputIndex] = NeuralMath.RandomRange(random, -0.5f, 0.5f);
                }
            });

        }
    }
}
