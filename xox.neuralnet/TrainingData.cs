using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet
{
    public class TrainingData
    {
        public double[] Input { get; private set; }
        public double[] Output { get; private set; }

        public TrainingData(int inputNodeCount, int outputNodeCount)
        {
            this.Input = new double[inputNodeCount];
            this.Output = new double[outputNodeCount];
        }

        public TrainingData(double[] inputData, double[] outputData)
        {
            this.Input = new double[inputData.Length];
            this.Output = new double[outputData.Length];

            Array.Copy(inputData, 0, this.Input, 0, inputData.Length);
            Array.Copy(outputData, 0, this.Output, 0, outputData.Length);
        }

    }
}
