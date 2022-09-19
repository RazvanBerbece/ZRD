using NUnit.Framework;

namespace ZRD.tests.Integration.Transaction
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
            string transactionString = System.IO.File.ReadAllText("../../../tests/Integration/Transaction/Transaction.json");
            
            // Check deserialization
            if (TransactionNS.Transaction.JsonStringToTransactionInstance(transactionString) is TransactionNS.Transaction transaction)
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