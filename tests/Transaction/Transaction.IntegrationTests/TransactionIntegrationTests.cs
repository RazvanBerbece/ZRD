using System.IO;
using TransactionNS;
using NUnit.Framework;
using WalletNS;

namespace TransactionIntegrationTestsNS
{
    public class TransactionIntegrationTests
    {

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.Progress.WriteLine("-- Testing Transaction Scenarios --\n");
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Transaction_CanBeDeserializedWithAllFields()
        {
            
            // Read Transaction string example
            string transactionString = System.IO.File.ReadAllText("../../../tests/Transaction/Transaction.IntegrationTests/Transaction.json");
            
            // Check deserialization
            if (Transaction.JsonStringToTransactionInstance(transactionString) is Transaction transaction)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail("Failed to deserialize transaction JSON string in scenario");
            }

        }
        
    }
}