using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet
{
    public class SimpleMatrix
    {
        public double[,] Values { get; protected set; }

        public SimpleMatrix(int xLength, int yLength, double? defaultValue = null)
        {
            this.Values = new double[xLength, yLength];
            if(defaultValue != null)
            {
                for (int xIndex = 0; xIndex < xLength; xIndex++)
                {
                    for (int yIndex = 0; yIndex < yLength; yIndex++)
                    {
                        this.Values[xIndex, yIndex] = defaultValue.Value;
                    }
                }
            }
        }
    }
}
