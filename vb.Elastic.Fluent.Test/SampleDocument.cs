using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vb.Elastic.Fluent.Objects.Document;

namespace vb.Elastic.Fluent.Test
{
    internal class NestedEntity
    {
        internal string Data { get; set; }
        internal int Code { get; set; }
    }
    [ElasticsearchType(Name = "sampledoc" , IdProperty = nameof(Id))]
    internal class SampleDocument:EsDocument
    {
        [Keyword]
        public string Id { get; set; }
        [Text]
        public string Title { get; set; }
        [Text]
        public string Content { get; set; }
        [Date]
        public DateTime Date { get; set; }
    }
}
