using NUnit.Framework;
using SearchifyEngine.Indexer;

namespace SearchifyEngine.Test
{
    public class ExtractDocTests
    {
        [Test]
        public void TestInvalidUrlDownload()
        {
            var path = ExtractDoc.Extract("2kaieke");
            Assert.IsNull(path);
        }

        [Test]
        public void TestValidUrlDownload()
        {
            var path = ExtractDoc.Extract("http://www.africau.edu/images/default/sample.pdf");
            Assert.IsNotNull(path);
            ExtractDoc.Delete(path);
            Assert.IsFalse(System.IO.File.Exists(path));
        }
    }
}