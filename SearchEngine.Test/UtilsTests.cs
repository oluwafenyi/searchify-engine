using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SearchEngine.Test
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void TestDeltaListConversion()
        {
            List<int> list = new List<int> {1, 2, 3, 4, 5};
            List<ulong> deltaList = Utils.ToDeltaList(list);
            CollectionAssert.AreEqual(new List<ulong> {1, 1, 1, 1, 1}, deltaList);

            list = new List<int> { 5, 84, 500, 3254 };
            deltaList = Utils.ToDeltaList(list);
            CollectionAssert.AreEqual(new List<ulong> {5, 79, 416, 2754}, deltaList);
        }
    }
}