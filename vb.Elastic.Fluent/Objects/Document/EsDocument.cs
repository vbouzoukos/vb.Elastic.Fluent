using Nest;
using System;
using System.Linq;
using vb.Elastic.Fluent.Core;
using vb.Elastic.Fluent.Indexer;

namespace vb.Elastic.Fluent.Objects.Document
{
    public abstract class EsDocument
    {
        /// <summary>
        /// The alias name of the index used in order to distinguish different versions of the same indez
        /// </summary>
        public virtual string IndexAlias
        {
            get
            {
                return string.Format("{0}-{1}", Manager.Instance.InstallationName, GetType().Name.ToLower());
            }
        }
        /// <summary>
        /// Current version's index name
        /// </summary>
        public virtual string DocumentIndex
        {
            get
            {
                return string.Format("{0}-{1}-{2}", Manager.Instance.InstallationName, GetType().Name.ToLower(), Manager.Instance.AppVersion);
            }
        }
        /// <summary>
        /// Creates the index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual void CreateIndex<T>() where T : EsDocument
        {
            string indexName = IndexAlias;
            var esClient = Manager.EsClient;

            if (!esClient.Indices.Exists(indexName).Exists)
            {
                createIndex<T>(DocumentIndex);
                esClient.Indices.PutAlias(DocumentIndex, indexName);// Alias(x => x.Add(a => a.Alias(indexName).Index(DocumentIndex)));
            }
        }
        /// <summary>
        /// TODO Define analysers used by the index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexSettings"></param>
        internal virtual void IndexAnalysers<T>(IndexSettings indexSettings)
        {
            if (AnalysisInfo.Instance.Analyzers != null && AnalysisInfo.Instance.Analyzers.Keys != null)
            {
                foreach (var key in AnalysisInfo.Instance.Analyzers.Keys)
                {
                    var custom = AnalysisInfo.Instance.Analyzers[key];
                    indexSettings.Analysis.Analyzers.Add(key, custom);
                }
            }
        }
        /// <summary>
        /// Maintenance call, used in order to update the current index into a new version
        /// </summary>
        /// <param name="removeOldIndex">If true older index get deleted</param>
        /// <param name="errorLogger">Action used by the user to parse error messages during reindexing</param>
        public virtual void ReIndex<T>(bool removeOldIndex = true, Action<string> errorLogger = null) where T : EsDocument
        {
            bool hasErrors = false;
            var esClient = Manager.EsClient;
            var currentIndexname = esClient.GetIndexByAlias(IndexAlias);
            //var currentIndexname = index.Indices.Keys.FirstOrDefault();
            if (currentIndexname == DocumentIndex)
            {
                throw new Exception("Trying to reindex same Index");
            }
            //Create new index mapping
            createIndex<T>(currentIndexname);
            //start reindexing
            var reindex = esClient.Reindex<T>(currentIndexname, DocumentIndex, q => q.MatchAll());
            var o = new ReindexObserver(onError: e =>
            {
                hasErrors = true;
                if (errorLogger != null)
                {
                    var errorMsg = string.Format("Failed to reindex:\r\n{0}\r\nDetails:\r\n{1}\r\n{2}\r\n{3}", e.Data, e.Message, e.InnerException, e.StackTrace);
                    errorLogger(errorMsg);
                };
            });
            reindex.Subscribe(o);
            //Change allias pointing
            esClient.Indices.PutAlias(DocumentIndex, IndexAlias);
            //esClient.Indices.PutAlias(DocumentIndex, IndexAlias);
            //Alias(a => a
            //.Add(aa => aa.Alias(IndexAlias).Index(DocumentIndex))
            //.Remove(aa => aa.Alias(IndexAlias).Index(currentIndexname.Name)));
            // Add completion flag            
            if (!hasErrors)
            {// Delete if successfull old index 
                if (removeOldIndex)
                {
                    esClient.Indices.Delete(new DeleteIndexRequest(currentIndexname));
                }
            }
        }

        private void createIndex<T>(string indexName) where T : EsDocument
        {
            var esClient = Manager.EsClient;
            IndexSettings indexSettings = new IndexSettings();
            //set up custom analyzers
            IndexAnalysers<T>(indexSettings);
            IndexState indexConfig = new IndexState
            {
                Settings = indexSettings
            };
            esClient.Indices
                .Create(indexName, c => c
                    .InitializeUsing(indexConfig)
                    .Map(m=>m.AutoMap<T>())
                    );
        }
    }
}
