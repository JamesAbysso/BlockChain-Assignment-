using BlockChainAssignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using static BlockChainAssignment.BlockList;

namespace ChainTest
{
    [TestClass]
    public class UnitTest1
    {
        BlockList TestObject;

        [TestInitialize]
        public void SetBlockChainToInitialState()
        {
            TestObject = GetInitializedBlockChain();
            string[] testData = new string[] { "x", "a", "b", "c", "d" };
            /* Set data */
            for (int i = 0; i < 5; i++)
            {
                TestObject.BlockChain[i].Data = testData[i];
            }
        }

        public void IsMined()
        {
            foreach (int i in Enumerable.Range(0, 5))
            {
                if (TestObject.BlockChain[i] is Block B)
                {
                    if (!B.IsSigned)
                    {
                        Task.Factory.StartNew(() => B.Mine()).Wait();
                    }
                }
            }
        }

        [TestMethod]
        public void NonceCheck()
        {
            string[] testNonces = new string[] { "1615", "11008", "32649", "50744", "133232" };

            IsMined();
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(TestObject.BlockChain[i].Nonce, testNonces[i]);
            }
        }

        [TestMethod]
        public void PrevHashCheck()
        {
            IsMined();
            Assert.AreEqual(TestObject.BlockChain[0].PrevHash, "0000000000000000000000000000000000000000");
            for (int i = 1; i < 5; i++)
            {
                Assert.AreEqual(TestObject.BlockChain[i].PrevHash, TestObject.BlockChain[i - 1].Hash);
            }
        }

        [TestMethod]
        public void IDCheck()
        {
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(TestObject.BlockChain[i].ID, i.ToString());
            }
        }

        [TestMethod]
        public void SignedCheck()
        {
            IsMined();
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(TestObject.BlockChain[i].IsSigned, true);
            }
        }

        [TestMethod]
        public void HashLengthCheck()
        {
            IsMined();
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(TestObject.BlockChain[i].Hash.Length, 40);
            }
        }
    }
}
