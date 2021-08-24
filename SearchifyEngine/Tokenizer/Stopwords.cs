using System.Collections.Generic;

namespace SearchifyEngine.Tokenizer
{
    public static class Stopwords
    {
        public static HashSet<string> LoadStopWords()
        {
            string stopwords = "i me my myself we our ours ourselves you youre youve youll youd your yours yourself yourselves he him his himself she shes her hers herself it its its itself they them their theirs themselves what which who whom this that thatll these those am is are was were be been being have has had having do does did doing a an the and but if or because as until while of at by for with about against between into through during before after above below to from up down in out on off over under again further then once here there when where why how all any both each few more most other some such no nor not only own same so than too very s t can will just don dont should shouldve now d ll m o re ve y ain aren arent couldn couldnt didn didnt doesn doesnt hadn hadnt hasn hasnt haven havent isn isnt ma mightn mightnt mustn mustnt needn neednt shan shant shouldn shouldnt wasn wasnt weren werent wont wouldn wouldnt";
            return new HashSet<string>(stopwords.Split(' '));
        }
    }
}