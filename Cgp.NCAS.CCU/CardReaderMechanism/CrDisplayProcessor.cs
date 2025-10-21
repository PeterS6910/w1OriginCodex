using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    public class CrDisplayProcessor
    {
        public const int MaxDisplayCountOfChar = 24;

        private readonly ACardReaderSettings _cardReaderSettings;

        internal CrDisplayProcessor(ACardReaderSettings cardReaderSettings)
        {
            _cardReaderSettings = cardReaderSettings;
        }

        private static IEnumerable<string> SplitByEnterChar(string text)
        {
            if (String.IsNullOrEmpty(text))
                yield break;

            for (int i = 0; i < text.Length; i++)
            {
                int found = text.IndexOf("\\n", i);

                if (found > -1
                    && found - i > -1)
                {
                    yield return text.Substring(i, found - i);
                    i = found + 1;

                    continue;
                }

                yield return text.Substring(i, text.Length - i);
                break;
            }
        }

        public static LinkedList<string> GetStringLinesForDisplay(string fullText)
        {
            if (String.IsNullOrEmpty(fullText))
                return null;

            var lines = new LinkedList<string>();
            var fullTextParts = SplitByEnterChar(fullText);

            foreach (var text in fullTextParts)
            {
                var rowCount = text.Length/MaxDisplayCountOfChar;

                if (text.Length%MaxDisplayCountOfChar > 0)
                    rowCount++;

                var words = text.Split(' ');

                var correctWords = new LinkedList<string>();

                foreach (var word in words)
                {
                    if (word.Length <= MaxDisplayCountOfChar)
                    {
                        correctWords.AddLast(word);
                        continue;
                    }

                    var count = word.Length/MaxDisplayCountOfChar;

                    if (word.Length%MaxDisplayCountOfChar > 0)
                        count++;

                    for (var i = 0; i < count; i++)
                    {
                        var lenght =
                            i*MaxDisplayCountOfChar + MaxDisplayCountOfChar >
                            word.Length
                                ? word.Length - i*MaxDisplayCountOfChar
                                : MaxDisplayCountOfChar;

                        correctWords.AddLast(
                            word.Substring(
                                i*MaxDisplayCountOfChar,
                                lenght));
                    }
                }

                var line = String.Empty;

                foreach (var word in correctWords)
                {
                    if ((line.Length == 0 ? line.Length : line.Length + 1) + word.Length >
                        MaxDisplayCountOfChar)
                    {
                        lines.AddLast(line);
                        line = String.Empty;
                    }

                    if (line.Length > 0)
                        line += " ";

                    line += word;
                }

                if (line != null)
                    lines.AddLast(line);
            }

            return lines;
        }

        public string GetLocalizationString(string text)
        {
            return _cardReaderSettings.GetLocalizationString(text);
        }

        public byte DisplayWriteText(string text, byte left, byte top)
        {
            if (String.IsNullOrEmpty(text))
                return top;

            var sentences = GetStringLinesForDisplay(text);

            if (sentences == null)
                return top;

            foreach (string sentence in sentences)
                _cardReaderSettings.DisplayText(
                    left,
                    top++,
                    sentence);

            return top;
        }
    }
}
