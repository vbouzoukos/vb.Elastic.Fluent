using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vb.Elastic.Fluent.Core
{
    internal class QuoteSplitResult
    {
        public bool HasQuotes { get; set; }
        public string Token { get; set; }
    }
    internal class Parser
    {
        /// <summary>
        /// Splits the string passed in by the delimiters passed in.
        /// Quoted sections are not split, and all tokens have whitespace
        /// trimmed from the start and end.
        internal static List<QuoteSplitResult> SplitQuotes(string source, params char[] delimiters)
        {
            List<QuoteSplitResult> results = new List<QuoteSplitResult>();

            bool inQuote = false;
            bool hasQuote = false;
            StringBuilder currentToken = new StringBuilder();
            for (int index = 0; index < source.Length; ++index)
            {
                char currentCharacter = source[index];
                if (currentCharacter == '"')
                {
                    inQuote = !inQuote;
                    hasQuote = true;
                }
                else if (delimiters.Contains(currentCharacter) && inQuote == false)
                {
                    string result = currentToken.ToString().Trim();
                    if (result != "")
                    {
                        QuoteSplitResult token = new QuoteSplitResult { HasQuotes = hasQuote, Token = result };
                        results.Add(token);
                    }
                    hasQuote = false;
                    currentToken = new StringBuilder();
                }
                else
                {
                    currentToken.Append(currentCharacter);
                }
            }
            string lastResult = currentToken.ToString().Trim();
            if (lastResult != "")
            {
                QuoteSplitResult lasttoken = new QuoteSplitResult { HasQuotes = hasQuote, Token = lastResult };
                results.Add(lasttoken);
            }
            return results;
        }
    }
}
