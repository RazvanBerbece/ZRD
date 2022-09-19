using System;
using NUnit.Framework;

namespace ZRD.tests.Unit.Statics;

[TestFixture]
public class StaticsTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TestContext.Progress.WriteLine("-- Testing Statics --\n");
    }

    [TestCase("")]
    [TestCase("dataToEncode")]
    public void Can_CreateSHA256HashString(string data)
    {
        Assert.Throws<ArgumentNullException>(() => StaticsNS.Statics.CreateHashSha256(null));
        Assert.That(StaticsNS.Statics.CreateHashSha256(data), Is.Not.Null);
        Assert.That(StaticsNS.Statics.CreateHashSha256(data), Has.Count.GreaterThan(0));
    }
}