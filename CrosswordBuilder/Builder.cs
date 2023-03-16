using System.Text.RegularExpressions;

namespace CrosswordBuilder
{
    public class Builder
    {
        private enum Available
        {
            True,
            False,
            ShortSpace
        }

        public char[][] Crossword { get; set; }

        public bool AddNewWord(string newWord)
        {
            if (Crossword is null)
            {
                Crossword = new char[1][];
                Crossword[0] = newWord.ToCharArray();
                return true;
            }

            Dictionary<(sbyte, sbyte), char> crosswordCommon = new();
            Dictionary<sbyte, char> wordCommon = new();

            for (sbyte i = 0; i < Crossword.GetLength(0); i++)
                for (sbyte j = 0; j < Crossword[i].Length; j++)
                    if (newWord.Contains(Crossword[i][j]))
                        crosswordCommon.Add((i, j), Crossword[i][j]);

            for (sbyte i = 0; i < newWord.Length; i++)
                if (crosswordCommon.Values.Contains(newWord[i]))
                    wordCommon.Add(i, newWord[i]);

            var r = new Random();
            foreach (var cc in crosswordCommon.OrderBy(w => r.Next()))
                foreach (var wc in wordCommon.OrderBy(w => r.Next()))
                    if (wc.Value == cc.Value)
                        if (FitsHorizontally(cc.Key, newWord, wc.Key) || FitsVertically(cc.Key, newWord, wc.Key))
                            return true;

            return false;
        }

        public (sbyte, sbyte) GetStartIdxs(string word)
        {
            for (sbyte i = 0; i < Crossword.Length; i++)
            {

                if (new string(Crossword[i]).Contains(word))
                {
                    sbyte startIdx = (sbyte)new string(Crossword[i]).IndexOf(word);
                    if ((startIdx - 1 < 0 || IsNonAlpha(Crossword[i][startIdx - 1]))
                        && (startIdx + word.Length >= Crossword[i].Length - 1
                        || IsNonAlpha(Crossword[i][startIdx + word.Length])))
                        return (i, startIdx);
                }
            }
            for (sbyte i = 0; i <= Crossword.Length - word.Length; i++)
                for (sbyte j = 0; j < Crossword[i].Length; j++)
                {
                    if (Crossword[i][j] == word[0] && i + word.Length - 1 < Crossword.GetLength(0) &&
                        (i - 1 < 0 || IsNonAlpha(Crossword[i - 1][j]))
                        && (i + word.Length == Crossword.GetLength(0) || IsNonAlpha(Crossword[i + word.Length][j])))
                        for (sbyte k = i, w = 0; k < Crossword.GetLength(0) && w < word.Length; k++, w++)
                        {
                            if (!word[w].Equals(Crossword[k][j]))
                                break;
                            if (w == word.Length - 1)
                                return (i, j);
                        }
                }
            throw new IndexOutOfRangeException($"{word}");
        }

        private bool IsNonAlpha(char c) => Regex.IsMatch(c.ToString(), @"[^a-zA-Z]");

        private bool FitsHorizontally((sbyte, sbyte) Place, string wordToFit, sbyte idxToFit)
        {
            Available left = isAvailableOnLeft(Place, wordToFit, idxToFit);
            if (left == Available.False)
                return false;

            Available right = isAvailableOnRight(Place, wordToFit, idxToFit);
            if (right == Available.False)
                return false;

            if (right == Available.ShortSpace)
                InsertRight((sbyte)(wordToFit.Length - 1 - idxToFit - (Crossword[Place.Item1].Length - 1 - Place.Item2)));
            if (left == Available.ShortSpace)
            {
                InsertLeft((sbyte)(idxToFit - Place.Item2));
                wordToFit.CopyTo(0, Crossword[Place.Item1], 0, wordToFit.Length);
                return true;
            }
            for (sbyte j = (sbyte)(Place.Item2 - idxToFit), k = 0; j < Crossword[Place.Item1].Length && k < wordToFit.Length; j++, k++)
            {
                Crossword[Place.Item1][j] = wordToFit[k];
            }
            return true;
        }

        private bool FitsVertically((sbyte, sbyte) Place, string wordToFit, sbyte idxToFit)
        {
            var down = isAvailableDown(Place, wordToFit, idxToFit);
            if (down == Available.False)
                return false;

            var up = isAvailableUp(Place, wordToFit, idxToFit);
            if (up == Available.False)
                return false;

            if (down == Available.ShortSpace)
                InsertBottom((sbyte)(wordToFit.Length - idxToFit - (Crossword.GetLength(0) - Place.Item1)));
            if (up == Available.ShortSpace)
            {
                InsertTop((sbyte)(idxToFit - Place.Item1));
                for (sbyte i = 0; i < wordToFit.Length; i++)
                    Crossword[i][Place.Item2] = wordToFit[i];
                return true;
            }
            for (sbyte i = (sbyte)(Place.Item1 - idxToFit), k = 0; k < wordToFit.Length; i++, k++)
            {
                Crossword[i][Place.Item2] = wordToFit[k];
            }
            return true;
        }

        private Available isAvailableOnLeft((sbyte, sbyte) Place, string wordToFit, sbyte idxToFit)
        {
            if (Place.Item2 - idxToFit - 1 >= 0 && !IsNonAlpha(Crossword[Place.Item1][Place.Item2 - idxToFit - 1]))
                return Available.False;

            if (idxToFit > 0 && Place.Item2 - 1 > 0 && Place.Item2 + 1 < Crossword[Place.Item1].Length && Place.Item1 - 1 > 0)
                if (!IsNonAlpha(Crossword[Place.Item1 - 1][Place.Item2 - 1])
                    || !IsNonAlpha(Crossword[Place.Item1 - 1][Place.Item2 + 1]))
                    return Available.False;

            for (sbyte m = Place.Item2, n = idxToFit; n >= 0 && m >= 0; m--, n--)
            {
                if ((m != Place.Item2 && Place.Item1 - 1 >= 0 && !IsNonAlpha(Crossword[Place.Item1 - 1][m]))
                    || (m != Place.Item2 && Place.Item1 + 1 < Crossword.GetLength(0) && !IsNonAlpha(Crossword[Place.Item1 + 1][m])))
                    return Available.False;
                if (n > 0 && m == 0)
                    return Available.ShortSpace;
                if (m != Place.Item1 && !IsNonAlpha(Crossword[Place.Item1][m]) && Crossword[Place.Item1][m] != wordToFit[n])
                    return Available.False;
            }
            return Available.True;
        }

        private Available isAvailableOnRight((sbyte, sbyte) Place, string wordToFit, sbyte idxToFit)
        {
            if (idxToFit < wordToFit.Length - 1 && Place.Item2 - 1 > 0
                && Place.Item2 + 1 < Crossword[Place.Item1].Length && Place.Item1 + 1 < Crossword.GetLength(0))

                if (!IsNonAlpha(Crossword[Place.Item1 + 1][Place.Item2 - 1])
                    || !IsNonAlpha(Crossword[Place.Item1 + 1][Place.Item2 + 1]))
                    return Available.False;

            for (sbyte m = Place.Item2, n = idxToFit; n <= wordToFit.Length; m++, n++)
            {
                if ((m != Place.Item2 && Place.Item1 - 1 >= 0 && !IsNonAlpha(Crossword[Place.Item1 - 1][m]))
                    || (m != Place.Item2 && Place.Item1 + 1 < Crossword.GetLength(0) && !IsNonAlpha(Crossword[Place.Item1 + 1][m])))
                    return Available.False;
                if (n < wordToFit.Length && m == Crossword[Place.Item1].Length - 1)
                    return Available.ShortSpace;
                if (n == wordToFit.Length && !IsNonAlpha(Crossword[Place.Item1][m]))
                    return Available.False;
                if (n < wordToFit.Length && !IsNonAlpha(Crossword[Place.Item1][m]) && Crossword[Place.Item1][m] != wordToFit[n])
                    return Available.False;
            }
            return Available.True;
        }

        private Available isAvailableUp((sbyte, sbyte) Place, string wordToFit, sbyte idxToFit)
        {
            if (Place.Item1 - idxToFit - 1 >= 0 && !IsNonAlpha(Crossword[Place.Item1 - idxToFit - 1][Place.Item2]))
                return Available.False;

            if (idxToFit > 0 && Place.Item1 + 1 < Crossword.GetLength(0)
                && Place.Item2 + 1 < Crossword[Place.Item1].Length && Place.Item1 - 1 >= 0 && Place.Item2 - 1 >= 0)
                if (!IsNonAlpha(Crossword[Place.Item1 - 1][Place.Item2 - 1]) || !IsNonAlpha(Crossword[Place.Item1 - 1][Place.Item2 + 1]))
                    return Available.False;

            for (sbyte m = Place.Item1, n = idxToFit; n >= 0 && m >= 0; m--, n--)
            {
                if (m != Place.Item1)
                    if ((Place.Item2 - 1 >= 0 && !IsNonAlpha(Crossword[m][Place.Item2 - 1]))
                        || (Place.Item2 + 1 < Crossword[m].Length && !IsNonAlpha(Crossword[m][Place.Item2 + 1])))
                        return Available.False;
                if (n > 0 && m == 0)
                    return Available.ShortSpace;
                if (Crossword[m][Place.Item2] != wordToFit[n] && !IsNonAlpha(Crossword[m][Place.Item2]))
                    return Available.False;
            }
            return Available.True;
        }

        private Available isAvailableDown((sbyte, sbyte) Place, string wordToFit, sbyte idxToFit)
        {
            if (idxToFit < wordToFit.Length - 1 && Place.Item2 - 1 > 0
                && Place.Item2 + 1 < Crossword[Place.Item1].Length && Place.Item1 + 1 < Crossword.GetLength(0))
                if (!IsNonAlpha(Crossword[Place.Item1 + 1][Place.Item2 - 1])
                    || !IsNonAlpha(Crossword[Place.Item1 + 1][Place.Item2 + 1]))
                    return Available.False;

            if (Place.Item1 + (wordToFit.Length - idxToFit) < Crossword.GetLength(0)
                && !IsNonAlpha(Crossword[Place.Item1 + (wordToFit.Length - idxToFit)][Place.Item2]))
                return Available.False;

            for (sbyte m = Place.Item1, n = idxToFit; n < wordToFit.Length; m++, n++)
            {
                if (m != Place.Item1)
                    if ((Place.Item2 - 1 >= 0 && !IsNonAlpha(Crossword[m][Place.Item2 - 1]))
                    || (Place.Item2 + 1 < Crossword[m].Length && !IsNonAlpha(Crossword[m][Place.Item2 + 1])))
                        return Available.False;
                if (m != Place.Item1 && m - 1 >= 0 && !IsNonAlpha(Crossword[m][Place.Item2]))
                    return Available.False;
                if (n < wordToFit.Length && m >= Crossword.GetLength(0) - 1)
                    return Available.ShortSpace;
                if (Crossword[m][Place.Item2] != wordToFit[n] && !IsNonAlpha(Crossword[m][Place.Item2]))
                    return Available.False;
            }
            return Available.True;
        }

        private void InsertLeft(sbyte colNum)
        {
            char[] newRow;

            for (sbyte i = 0; i < Crossword.GetLength(0); i++)
            {
                newRow = new char[colNum + Crossword[i].Length];
                Crossword[i].CopyTo(newRow, colNum);
                Crossword[i] = newRow;
            }
        }

        private void InsertRight(sbyte colNum)
        {
            char[] newRow;

            for (sbyte i = 0; i < Crossword.GetLength(0); i++)
            {
                newRow = new char[colNum + Crossword[i].Length];
                Crossword[i].CopyTo(newRow, 0);
                Crossword[i] = newRow;
            }
        }

        private void InsertTop(sbyte rowNum)
        {
            char[][] newArray = new char[Crossword.GetLength(0) + rowNum][];

            for (sbyte i = 0; i < newArray.Length; i++)
            {
                if (i >= rowNum)
                    newArray[i] = Crossword[i - rowNum];
                else newArray[i] = new char[Crossword[0].Length];
            }
            Crossword = newArray;
        }

        private void InsertBottom(sbyte rowNum)
        {
            char[][] newArray = new char[Crossword.GetLength(0) + rowNum][];

            for (int i = 0; i < newArray.Length; i++)
            {
                if (i >= newArray.Length - rowNum)
                    newArray[i] = new char[Crossword[0].Length];
                else
                    newArray[i] = Crossword[i];
            }
            Crossword = newArray;
        }
    }
}
