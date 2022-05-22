using System.Collections.Generic;
using NUnit.Framework;
using BlockNS;
using TransactionNS;
using System.Text.RegularExpressions;
using WalletNS;

namespace WalletTestsNS
{

    public class WalletUnitTests
    {

        // Generic values which are Setup for every test
        private Wallet wallet;

        [OneTimeSetUp]
        public void Setup()
        {
            this.wallet = new Wallet(1024);
        }

        [Test]
        public void Wallet_CanConstruct()
        {
            Assert.IsNotEmpty(this.wallet.publicKey);
            Assert.IsNotEmpty(this.wallet.GetPrivateKey());
        }

        [Test]
        public void External_PrivateMemberAccess()
        {
            string privateKey = this.wallet.GetPrivateKey();
            Assert.IsNotEmpty(privateKey);
        }

    }

}
