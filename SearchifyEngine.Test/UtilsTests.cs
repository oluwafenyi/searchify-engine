using System.Collections.Generic;
using NUnit.Framework;

namespace SearchifyEngine.Test
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void TestCleanText()
        {
            string dirty = "Hello % world  . This text is dirty !";
            Assert.AreEqual("hello world this text is dirty", Utils.CleanText(dirty));
        }
        
        [Test]
        public void TestDeltaListConversion()
        {
            List<uint> list = new List<uint> {1, 2, 3, 4, 5};
            List<uint> deltaList = Utils.ToDeltaList(list);
            CollectionAssert.AreEqual(new List<ulong> {1, 1, 1, 1, 1}, deltaList);

            list = new List<uint> { 5, 84, 500, 3254 };
            deltaList = Utils.ToDeltaList(list);
            CollectionAssert.AreEqual(new List<ulong> {5, 79, 416, 2754}, deltaList);
        }
    }
}