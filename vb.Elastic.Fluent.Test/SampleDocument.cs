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
    [ElasticsearchType(RelationName = "sampledoc", IdProperty = nameof(Id))]
    internal class SampleDocument : EsDocument
    {
        [Keyword]
        public string Id { get; set; }
        [Number(Name = "sort")]
        public int Sort { get; set; }
        [Text]
        public string Title { get; set; }
        [Text]
        public string Content { get; set; }
        [Date(Name = "docdate")]
        public DateTime DocDate { get; set; }
    }
}
