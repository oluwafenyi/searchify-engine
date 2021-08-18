using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SearchEngine.Tokenizer
{
    public class PorterStemmer
    {
        // This function returns true if a letter is a consonant otherwise false.
        private bool IsCons(char letter)
        {
            return !new List<char>{'a', 'e', 'i', 'o', 'u'}.Contains(letter);
        }

        // This function returns true only if the letter at i th position 
        // in the argument 'word' is a consonant. But if the letter is 'y' and the letter at i-1 th position 
        // is also a consonant, then it returns false.
        private bool IsConsonant(string word, int i)
        {
            char letter = word[i];
            if (IsCons(letter))
            {
                if (letter == 'y' && i > 1 && IsCons(word[i - 1]))
                {
                    return false;
                }

                if (letter == 'y' && i == 0 && IsCons(word[word.Length - 1]))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        // This function returns true if the letter at i th position in the argument 'word'
        // is a vowel.
        private bool IsVowel(string word, int i)
        {
            return !IsConsonant(word, i);
        }

        // // This function returns true if the word 'stem' ends with 'letter'.
        // private bool EndsWith(string stem, char letter)
        // {
        //     return stem[stem.Length - 1] == letter;
        // }
        //
        // // returns true if 'stem' ends with 'substr'
        // private bool EndsWith(string stem, string substr)
        // {
        //     return stem.EndsWith(substr);
        // }
        
        // This function returns true if the word 'stem' contains a vowel.
        private bool ContainsVowel(string stem)
        {
            foreach (var c in stem.ToCharArray())
            {
                if (!IsCons(c))
                {
                    return true;
                }
            }

            return true;
        }
        
        // This function returns true if the word 'stem' ends with 2 consonants.
        private bool DoubleCons(string stem)
        {
            if (stem.Length >= 2)
            {
                return IsConsonant(stem, stem.Length - 1) && IsConsonant(stem, stem.Length - 2);
            }

            return false;
        }
        
        // This function takes a word as an input, and checks for vowel and consonant sequences in that word.
        // vowel sequence is denoted by V and consonant sequences by C
        // For example, the word 'balloon' can be divived into following sequences:
        // 'b' : C
        // 'a' : V
        // 'll': C
        // 'oo': V
        // 'n' : C
        // So form = [C,V,C,V,C] and formstr = CVCVC
        private string GetForm(string word)
        {
            List<char> form = new List<char>();
            char prev;
            
            for (int i = 0; i < word.Length; i++)
            {
                if (IsConsonant(word, i))
                {
                    if (i != 0)
                    {
                        prev = form[form.Count - 1];
                        if (prev != 'C')
                        {
                            form.Add('C');
                        }
                    }
                    else
                    {
                        form.Add('C');
                    }
                }
                else
                {
                    if (i != 0)
                    {
                        prev = form[form.Count - 1];
                        if (prev != 'V')
                        {
                            form.Add('V');
                        }
                    }
                    else
                    {
                        form.Add('V');
                    }
                }
            }

            return new string(form.ToArray());
        }

        // This function returns value of M which is equal to number of 'VC' in formstr 
        // So in the word 'balloon', we have 2 'VC'.
        private int GetM(string word)
        {
            string form = GetForm(word);
            return Regex.Matches(form, "VC").Count;
        }
        
        // This function returns true if the last 3 letters of the word are of the following pattern: consonant,vowel,consonant
        // but if the last word is either 'w','x' or 'y', it returns false.
        private bool CVC(string word)
        {
            if (word.Length >= 3)
            {
                int f = word.Length - 3;
                int s = word.Length - 2;
                int t = word.Length - 1;

                char third = word[t];
                if (IsConsonant(word, f) && IsVowel(word, s) && IsConsonant(word, t))
                {
                    return !new List<char> { 'x', 'y', 'z' }.Contains(third);
                }

                return false;
            }

            return false;
        }

        // returns new str with 'rem' removed from 'orig'
        private string GetBaseString(string orig, string rem)
        {
            int lastIndex = orig.LastIndexOf(rem, StringComparison.Ordinal);
            if (lastIndex == -1)
            {
                lastIndex = orig.Length - 1;
            }
            
            return orig.Substring(0, lastIndex);
        }

        // This function checks if string 'orig' ends with 'rem' and
        // replaces 'rem' by the substring 'rep'. The resulting string 'replaced'
        // is returned.
        private string Replace(string orig, string rem, string rep)
        {
            string baseStr = GetBaseString(orig, rem);
            return baseStr + rep;
        }
        
        // This function is same as the function replace(), except that it checks the value of M for the 
        // base string. If it is greater than 0 , it replaces 'rem' by 'rep', otherwise it returns the
        // original string.
        private string ReplaceM0(string orig, string rem, string rep)
        {
            string baseStr = GetBaseString(orig, rem);
            if (GetM(baseStr) > 0)
            {
                return baseStr + rep;
            }

            return orig;
        }

        // This function is same as replaceM0(), except that it replaces 'rem' by 'rep', only when M>1 for
        // the base string.
        private string ReplaceM1(string orig, string rem, string rep)
        {
            string baseStr = GetBaseString(orig, rem);
            if (GetM(baseStr) > 1)
            {
                return baseStr + rep;
            }

            return orig;
        }

        // In a given word, this function replaces 'sses' by 'ss', 'ies' by 'i',
        // 'ss' by 'ss' and 's' by ''.
        //
        // step1a gets rid of plurals. e.g.
        //
        //     caresses  ->  caress
        //     ponies    ->  poni
        //     ties      ->  ti
        //     caress    ->  caress
        //     cats      ->  cat
        private string Step1A(string word)
        {
            if (word.EndsWith("sses"))
            {
                word = Replace(word, "sses", "ss");
            }
            else if (word.EndsWith("ies"))
            {
                word = Replace(word, "ies", "i");
            }
            else if (word.EndsWith("ss"))
            {
                word = Replace(word, "ss", "ss");
            }
            else if (word.EndsWith("s"))
            {
                word = Replace(word, "s", "");
            }

            return word;
        }

        // This function checks if a word ends with 'eed','ed' or 'ing' and replces these substrings by
        // 'ee','' and ''. If after the replacements in case of 'ed' and 'ing', the resulting word
        // -> ends with 'at','bl' or 'iz' : add 'e' to the end of the word
        // -> ends with 2 consonants and its last letter isn't 'l','s' or 'z': remove last letter of the word
        // -> has 1 as value of M and the cvc(word) returns true : add 'e' to the end of the word
        //
        //
        // step1b gets rid of -eed -ed or -ing. e.g.
        //
        //     feed      ->  feed
        //     agreed    ->  agree
        //     disabled  ->  disable
        //
        //     matting   ->  mat
        //     mating    ->  mate
        //     meeting   ->  meet
        //     milling   ->  mill
        //     messing   ->  mess
        //
        //     meetings  ->  meet
        private string Step1B(string word)
        {
            bool flag = false;

            if (word.EndsWith("eed"))
            {
                string baseStr = GetBaseString(word, "eed");
                if (GetM(baseStr) > 0)
                {
                    word = baseStr + "ee";
                }
            }
            else if (word.EndsWith("ed"))
            {
                string baseStr = GetBaseString(word, "ed");
                if (ContainsVowel(baseStr))
                {
                    word = baseStr;
                    flag = true;
                }
            }
            else if (word.EndsWith("ing"))
            {
                string baseStr = GetBaseString(word, "ing");
                if (ContainsVowel(baseStr))
                {
                    word = baseStr;
                    flag = true;
                }
            }

            if (flag)
            {
                if (word.EndsWith("at") || word.EndsWith("bl") || word.EndsWith("iz"))
                {
                    word += "e";
                }
                else if (DoubleCons(word) && !word.EndsWith("l") && !word.EndsWith("s") && !word.EndsWith("z"))
                {
                    word = word.Substring(0, word.Length - 1);
                }
                else if (GetM(word) == 1 && CVC(word))
                {
                    word += "e";
                }
            }

            return word;
        }

        // In words ending with 'y', this function replaces 'y' by 'i'.
        //
        // step1c turns terminal y to i when there is another vowel in the stem."""        
        private string Step1C(string word)
        {
            if (word.EndsWith("y"))
            {
                string baseStr = GetBaseString(word, "y");
                if (ContainsVowel(baseStr))
                {
                    word = baseStr + "i";
                }
            }

            return word;
        }

        // This function checks the value of M, and replaces the suffixes accordingly
        //
        // step2 maps double suffices to single ones.
        // so -ization ( = -ize plus -ation) maps to -ize etc. note that the
        // string before the suffix must give m() > 0.        
        private string Step2(string word)
        {
            if (word.EndsWith("ational"))
            {
                word = ReplaceM0(word, "ational", "ate");
            }
            else if (word.EndsWith("tional"))
            {
                word = ReplaceM0(word, "tional", "tion");
            }
            else if (word.EndsWith("enci"))
            {
                word = ReplaceM0(word, "enci", "ence");
            }
            else if (word.EndsWith("anci"))
            {
                word = ReplaceM0(word, "anci", "ance");
            }
            else if (word.EndsWith("izer"))
            {
                word = ReplaceM0(word, "izer", "ize");
            }
            else if (word.EndsWith("abli"))
            {
                word = ReplaceM0(word, "abli", "able");
            }
            else if (word.EndsWith("alli"))
            {
                word = ReplaceM0(word, "alli", "al");
            }
            else if (word.EndsWith("entli"))
            {
                word = ReplaceM0(word, "entli", "ent");
            }
            else if (word.EndsWith("eli"))
            {
                word = ReplaceM0(word, "eli", "e");
            }
            else if (word.EndsWith("ousli"))
            {
                word = ReplaceM0(word, "ousli", "ous");
            }
            else if (word.EndsWith("ization"))
            {
                word = ReplaceM0(word, "ization", "ize");
            }
            else if (word.EndsWith("ation"))
            {
                word = ReplaceM0(word, "ation", "ate");
            }
            else if (word.EndsWith("ator"))
            {
                word = ReplaceM0(word, "ator", "ate");
            }
            else if (word.EndsWith("alism"))
            {
                word = ReplaceM0(word, "alism", "al");
            }
            else if (word.EndsWith("iveness"))
            {
                word = ReplaceM0(word, "iveness", "ive");
            }
            else if (word.EndsWith("fulness"))
            {
                word = ReplaceM0(word, "fulness", "ful");
            }
            else if (word.EndsWith("ousness"))
            {
                word = ReplaceM0(word, "ousness", "ous");
            }
            else if (word.EndsWith("aliti"))
            {
                word = ReplaceM0(word, "aliti", "al");
            }
            else if (word.EndsWith("iviti"))
            {
                word = ReplaceM0(word, "iviti", "ive");
            }
            else if (word.EndsWith("biliti"))
            {
                word = ReplaceM0(word, "biliti", "ble");
            }

            return word;
        }

        // This function checks the value of M, and replaces the suffixes accordingly.
        //
        // step3 dels with -ic-, -full, -ness etc. similar strategy to step2.        
        private string Step3(string word)
        {
            if (word.EndsWith("icate"))
            {
                word = ReplaceM0(word, "icate", "ic");
            }
            else if (word.EndsWith("ative"))
            {
                word = ReplaceM0(word, "ative", "");
            }
            else if (word.EndsWith("alize"))
            {
                word = ReplaceM0(word, "alize", "al");
            }
            else if (word.EndsWith("iciti"))
            {
                word = ReplaceM0(word, "iciti", "ic");
            }
            else if (word.EndsWith("ful"))
            {
                word = ReplaceM0(word, "ful", "");
            }
            else if (word.EndsWith("ness"))
            {
                word = ReplaceM0(word, "ness", "");
            }
            
            return word;
        }

        private string Step4(string word)
        {
            if (word.EndsWith("al"))
            {
                word = ReplaceM1(word, "al", "");
            }
            else if (word.EndsWith("ance"))
            {
                word = ReplaceM1(word, "ance", "");
            }
            else if (word.EndsWith("ence"))
            {
                word = ReplaceM1(word, "ence", "");
            }
            else if (word.EndsWith("er"))
            {
                word = ReplaceM1(word, "er", "");
            }
            else if (word.EndsWith("ic"))
            {
                word = ReplaceM1(word, "ic", "");
            }
            else if (word.EndsWith("able"))
            {
                word = ReplaceM1(word, "able", "");
            }
            else if (word.EndsWith("ible"))
            {
                word = ReplaceM1(word, "ible", "");
            }
            else if (word.EndsWith("ant"))
            {
                word = ReplaceM1(word, "ant", "");
            }
            else if (word.EndsWith("ement"))
            {
                word = ReplaceM1(word, "ement", "");
            }
            else if (word.EndsWith("ment"))
            {
                word = ReplaceM1(word, "ment", "");
            }
            else if (word.EndsWith("ent"))
            {
                word = ReplaceM1(word, "ent", "");
            }
            else if (word.EndsWith("ou"))
            {
                word = ReplaceM1(word, "ou", "");
            }
            else if (word.EndsWith("ism"))
            {
                word = ReplaceM1(word, "ism", "");
            }
            else if (word.EndsWith("ate"))
            {
                word = ReplaceM1(word, "ate", "");
            }
            else if (word.EndsWith("iti"))
            {
                word = ReplaceM1(word, "iti", "");
            }
            else if (word.EndsWith("ous"))
            {
                word = ReplaceM1(word, "ous", "");
            }
            else if (word.EndsWith("ive"))
            {
                word = ReplaceM1(word, "ive", "");
            }
            else if (word.EndsWith("ize"))
            {
                word = ReplaceM1(word, "ize", "");
            }
            else if (word.EndsWith("ion"))
            {
                string baseStr = GetBaseString(word, "ion");
                if (GetM(baseStr) > 1 && baseStr.EndsWith("s") || baseStr.EndsWith("t"))
                {
                    word = baseStr;
                }

                word = ReplaceM1(word, "", "");
            }
            
            return word;
        }

        // This function checks if the word ends with 'e'. If it does, it checks the value of
        // M for the base word. If M>1, OR, If M = 1 and cvc(base) is false, it simply removes 'e'
        // ending.
        //
        // step5 removes a final -e if m() > 1.        
        private string Step5A(string word)
        {
            if (word.EndsWith("e"))
            {
                string baseStr = word.Substring(0, word.Length - 1);
                
                if (GetM(baseStr) > 1)
                {
                    word = baseStr;
                }
                else if (GetM(baseStr) == 1 && !CVC(baseStr))
                {
                    word = baseStr;
                }
            }
            return word;
        }

        // This function checks if the value of M for the word is greater than 1 and it ends with 2 consonants
        // and it ends with 'l', it removes 'l'
        //
        // step5b changes -ll to -l if m() > 1        
        private string Step5B(string word)
        {
            if (GetM(word) > 1 && DoubleCons(word) && word.EndsWith("l"))
            {
                word = word.Substring(0, word.Length - 1);
            }
            return word;
        }

        // This functions puts together all the steps in porter stemming.
        public string StemWord(string word)
        {
            word = word.ToLower();
            word = Step1A(word);
            word = Step1B(word);
            word = Step1C(word);
            word = Step2(word);
            word = Step3(word);
            word = Step4(word);
            word = Step5A(word);
            word = Step5B(word);
            return word;
        }
    }
}