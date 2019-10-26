using Nest;
using System;
using System.Collections.Generic;

namespace vb.Elastic.Fluent.Indexer
{
    /// <summary>
    /// TODO Custom analysers load here
    /// </summary>
    internal class AnalysisInfo
    {
        static AnalysisInfo _instance = null;
        //List of analyzers that will be used to analyse fields
        public Dictionary<string, CustomAnalyzer> Analyzers { get; set; }
        AnalysisInfo()
        {
            Analyzers = new Dictionary<string, CustomAnalyzer>();
        }
        public static AnalysisInfo Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AnalysisInfo();
                return _instance;
            }
        }
        public void AddAnalyser(string id, string tokenizer, string filter)
        {
            CustomAnalyzer custom = new CustomAnalyzer();
            var filters = filter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            custom.Filter = filters;
            custom.Tokenizer = tokenizer;
            Analyzers.Add(id, custom);
        }
    }
}
