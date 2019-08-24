using Nest;
using System;
using vb.Elastic.Fluent.Core;

namespace vb.Elastic.Fluent.Objects.Document
{
    /// <summary>
    /// Base class used to create Entities with attachment
    /// </summary>
    public abstract class EsAttachment : EsDocument
    {
        /// <summary>
        /// Content the base64 data used by the Attachment
        /// </summary>
        [Keyword]
        public string Content { get; set; }

        /// <summary>
        /// Attachment used by ingest in order to store attachment for indexing
        /// </summary>
        [Object]
        public Attachment Data { get; set; }

        [Ignore]
        internal string PipeLineName { get { return string.Format("{0}_attachments", GetType().Name); } }

        /// <summary>
        /// Create index for attachment entities
        /// </summary>
        public new void CreateIndex<T>() where T : EsAttachment
        {
            string indexName = IndexAlias;
            //string indexId = DocumentIndex;
            var esClient = Manager.EsClient;

            if (!esClient.Indices.Exists(indexName).Exists)
            {
                base.CreateIndex<T>();
                //Pipeline
                esClient.Ingest.PutPipeline(PipeLineName, p => p
                  .Description(string.Format("Document attachment pipeline for {0}", GetType()))
                  .Processors(pr => pr
                    .Attachment<T>(a => a
                      .Field(f => f.Content)
                      .TargetField(f => f.Data)
                    ).Remove<T>(r => r.Field(f => f.Field(t => t.Content))
                    )
                  )
                );
            }
        }
    }
}

