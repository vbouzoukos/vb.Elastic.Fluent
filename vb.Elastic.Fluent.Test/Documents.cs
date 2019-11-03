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
        [Keyword]
        internal string Data { get; set; }
        [Number]
        internal int Code { get; set; }
        [Date]
        internal DateTime Created { get; set; }
    }
    [ElasticsearchType(RelationName = "sampledoc", IdProperty = nameof(Id))]
    internal class SampleDocument : EsDocument
    {
        [Keyword]
        public string Id { get; set; }
        [Keyword]
        public string Sort { get; set; }
        [Text]
        public string Title { get; set; }
        [Text]
        public string Content { get; set; }
        [Date(Name = "docdate")]
        public DateTime DocDate { get; set; }
    }
    [ElasticsearchType(RelationName = "sampleweight")]
    internal class SampleWeight: EsDocument
    {
        [Number]
        public int Weight { get; set; }
    }
    [ElasticsearchType(RelationName = "sampleweight")]
    internal class SampleDate : EsDocument
    {
        [Date]
        public DateTime DocDate { get; set; }
    }

    [ElasticsearchType(RelationName = "sampleatt", IdProperty = nameof(Id))]
    internal class SampleAttachment : EsAttachment
    {
        [Keyword]
        public string Id { get; set; }
        [Text]
        public string Title { get; set; }
        [Date(Name = "docdate")]
        public DateTime Uploaded { get; set; }
    }
    [ElasticsearchType(RelationName = "sampleitm", IdProperty = nameof(Id))]
    internal class SampleItem:EsDocument
    {
        [Keyword]
        public string Id { get; set; }
        [Keyword]
        public string Code { get; set; }
        [Number]
        public int Sequence { get; set; }
    }
    [ElasticsearchType(RelationName = "sampleloc", IdProperty = nameof(Id))]
    internal class SampleLocation : EsDocument
    {
        [Keyword]
        public string Id { get; set; }
        [Text]
        public string Description { get; set; }
        [GeoPoint]
        public GeoLocation Location { get; set; }
    }
    [ElasticsearchType(RelationName = "samplenest", IdProperty = nameof(Id))]
    internal class SampleNest : EsDocument
    {
        [Keyword]
        public string Id { get; set; }
        [Nested]
        public IList<NestedEntity> Items { get; set; }
        [Text]
        public string Title { get; set; }

    }
}
