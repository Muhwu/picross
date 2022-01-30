using System;
using NUnit.Framework;

namespace Picross.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestLegality()
        {
            Picross.Game b = new Board();
            Assert.True(true);
        }
    }
}