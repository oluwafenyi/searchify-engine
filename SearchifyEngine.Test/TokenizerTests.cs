using System.Linq;
using NUnit.Framework;
using SearchifyEngine.Tokenizer;

namespace SearchifyEngine.Test
{
    public class TokenizerTests
    {
        [Test]
        public void TestStopwordsExtracted()
        {
            var stopwords = Stopwords.LoadStopWords();
            var paragraph = "Ten more steps. If he could take ten more steps it would be over, but his legs wouldn't move. " +
                            "He tried to will them to work, but they wouldn't listen to his brain. " +
                            "Ten more steps and it would be over but it didn't appear he would be able to do it.";
            var tokens = Tokenizer.Tokenizer.Tokenize(paragraph);
            
            var unfilteredStopwords = stopwords.Intersect(tokens);
            Assert.AreEqual(0, unfilteredStopwords.Count());
        }
        
        [Test]
        public void TestTokenizerDeterministic()
        {
            CollectionAssert.AreEqual(Tokenizer.Tokenizer.Tokenize("the quick brown fox"), Tokenizer.Tokenizer.Tokenize("the quick brown fox"));
        }
    }
}