using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet
{
    public static class NeuralMath
    {
        public static double HyperTan(double x)
        {
            if (x < -20d)
            {
                return -1d;
            }
            else if (x > 20d)
            {
                return 1d;
            }
            else
            {
                return Math.Tanh(x);
            }
        }

        //converts all nodes so that the highest value is always 1.0
        // does all output nodes at once so scale
        // doesn't have to be re-computed each time
        public static double[] Softmax(double[] oSums)
        {
            double sum = 0d;
            for (int i = 0; i < oSums.Length; ++i)
            {
                sum += Math.Exp(oSums[i]);
            }

            double[] result = new double[oSums.Length];
            for (int i = 0; i < oSums.Length; ++i)
            {
                result[i] = Math.Exp(oSums[i]) / sum;
            }
            return result;
        }

        public static int MaxIndex(double[] vector) // helper for Accuracy()
        {
            // index of largest value
            int index = 0;
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > vector[index])
                {
                    index = i;
                }
            }
            return index;
        }

        public static double RandomRange(Random rng, double min, double max)
        {
            return min + ((max - min) * rng.NextDouble());
        }

        //not really needed but keep parity with RandomRange for doubles
        public static int RandomRange(Random rng, int min, int max)
        {
            return rng.Next(min, max);
        }
       
    }
}
