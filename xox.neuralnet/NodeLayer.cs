using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet
{
    public class NodeLayer
    {
        public double[] Values { get; private set; }
        public double[] Biases { get; private set; }

        public NodeLayer(int nodeCount, bool hasBiases = true, double defaultNodeValue = 0d, double defaultBiasValue = 0d)
        {
            this.Values = new double[nodeCount];
            if(hasBiases == true)
            {
                this.Biases = new double[nodeCount];
            }

            for(int i = 0; i < nodeCount; i++)
            {
                this.Values[i] = defaultNodeValue;
                if (hasBiases == true)
                {
                    this.Biases[i] = defaultBiasValue;
                }
            }
        }

        public NodeLayer(NodeLayer source)
        {
            this.Values = new double[source.Values.Length];
            if(source.Biases != null)
            {
                this.Biases = new double[source.Biases.Length];
            }
            

            Array.Copy(source.Values, 0, this.Values, 0, this.Values.Length);
            if(source.Biases != null)
            {
                Array.Copy(source.Biases, 0, this.Biases, 0, this.Biases.Length);
            }

        }

    }
}
