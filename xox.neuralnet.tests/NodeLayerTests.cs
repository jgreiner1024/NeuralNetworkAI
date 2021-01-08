using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet.tests
{
    [TestClass]
    public class NodeLayerTests
    {
        [TestMethod]
        public void ItCanCreateNodeLayerAndSetDefaultValue()
        {
            double defaultNodeValue = 1.23456d;
            double defaultBiasValue = 0.00001d;

            NodeLayer layer = new NodeLayer(25, true, defaultNodeValue, defaultBiasValue);

            foreach (double value in layer.Values)
            {
                Assert.AreEqual(value, defaultNodeValue);
            }

            foreach (double bias in layer.Biases)
            {
                Assert.AreEqual(bias, defaultBiasValue);
            }
        }

        [TestMethod]
        public void ItCanCreateNodeLayerCopy()
        {
            Random rng = new Random();

            int nodeCount = 25;

            NodeLayer original = new NodeLayer(nodeCount, true);
            for (int i = 0; i < nodeCount; i++)
            {
                original.Values[i] = NeuralMath.RandomRange(rng, 0d, 1d);
                original.Biases[i] = NeuralMath.RandomRange(rng, 0d, 0.1d);
            }


            NodeLayer copy = new NodeLayer(original);
            for (int i = 0; i < nodeCount; i++)
            {
                Assert.AreEqual(original.Values[i], copy.Values[i]);
                Assert.AreEqual(original.Biases[i], copy.Biases[i]);
            }
        }
    }
}
