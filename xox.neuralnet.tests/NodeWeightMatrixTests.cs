using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xox.neuralnet.tests
{

    [TestClass]
    public class NodeWeightMatrixTests
    {
        const int inputNodeCount = 100;
        const int outputNodeCount = 50;

        [TestMethod]
        public void ItCanCreateNodeWeightMatrix()
        {
            NodeWeightMatrix weights = new NodeWeightMatrix(inputNodeCount, outputNodeCount);

            Assert.AreEqual(weights.Values.GetLength(0), inputNodeCount);
            Assert.AreEqual(weights.Values.GetLength(1), outputNodeCount);
        }

        [TestMethod]
        public void ItCanCreateNodeWeightMatrixCopy()
        {
            Random rng = new Random();

            NodeWeightMatrix original = new NodeWeightMatrix(inputNodeCount, outputNodeCount);
            original.Randomize(rng);


            NodeWeightMatrix copy = new NodeWeightMatrix(original);
            for(int inputIndex = 0; inputIndex < inputNodeCount; inputIndex++)
            {
                for (int outputIndex = 0; outputIndex < outputNodeCount; outputIndex++)
                {
                    Assert.AreEqual(original.Values[inputIndex, outputIndex], copy.Values[inputIndex, outputIndex]);
                }
            }
        }
    }
}
