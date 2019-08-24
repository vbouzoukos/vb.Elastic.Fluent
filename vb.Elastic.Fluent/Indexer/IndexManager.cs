using Nest;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using vb.Elastic.Fluent.Core;
using vb.Elastic.Fluent.Metadata;
using vb.Elastic.Fluent.Objects.Document;

namespace vb.Elastic.Fluent.Indexer
{
    /// <summary>
    /// This class is used in order to insert/update/delete data into the index
    /// </summary>
    public class IndexManager
    {
        #region Indexing
        /// <summary>
        /// Index an entity
        /// </summary>
        /// <param name="entity">The entity that will be updated</param>
        /// <param name="refresh">To enable the new entry to be available for next search (optional default:false)</param>
        public static void IndexEntity<T>(T entity, bool refresh = false) where T : EsDocument
        {
            var esClient = Manager.EsClient;
            entity.CreateIndex<T>();

            var response = esClient.Index(entity, p => p.Index(entity.IndexAlias));

            if (response.Result == Result.Error)
            {
                throw new Exception(response.OriginalException.Message);
            }
            if (refresh)
            {
                esClient.GetIndex<T>().Refresh(Elasticsearch.Net.Refresh.True);
            }
        }

        /// <summary>
        /// Used to index a document with a pipeline method
        /// </summary>
        /// <param name="entity">The entity that will be updated</param>
        /// <param name="pipeLineName">pipeline used during indexing</param>
        /// <param name="refresh">To enable the new entry to be available for next search (optional default:false)</param>
        public static void UpdateEntityPipeline<T>(T entity, string pipeLineName, bool refresh = false) where T : EsDocument
        {
            var esClient = Manager.EsClient;
            entity.CreateIndex<T>();

            var response = esClient.Index(entity, p => p.Index(entity.IndexAlias).Pipeline(pipeLineName));

            if (response.Result == Result.Error)
            {
                throw new Exception(response.OriginalException.Message);
            }
            if (refresh)
            {
                esClient.GetIndex<T>().Refresh(Elasticsearch.Net.Refresh.True);
            }
        }

        /// <summary>
        /// Used to updated an existing document
        /// </summary>
        /// <param name="pEntity">The document entity</param>
        /// <param name="pRefresh">Flag to refresh index state with new data</param>
        public static void UpdateEntity<T>(T pEntity, bool pRefresh = false) where T : EsDocument
        {
            var esClient = Manager.EsClient;
            pEntity.CreateIndex<T>();

            var response = esClient.Update<T, T>(pEntity, p => p.Doc(pEntity).Index(pEntity.IndexAlias));
            if (response.Result == Result.Error)
            {
                throw new Exception(response.OriginalException.Message);
            }
            if (pRefresh)
            {
                esClient.GetIndex<T>().Refresh(Elasticsearch.Net.Refresh.True);
            }
        }

        /// <summary>
        /// Deletes a document from the index
        /// </summary>
        /// <param name="pEntity">The document entity</param>
        /// <param name="pRefresh">Flag to refresh index state with new data</param>
        public static void DeleteEntity<T>(T pEntity, bool pRefresh = false) where T : EsDocument
        {
            var esClient = Manager.EsClient;
            pEntity.CreateIndex<T>();
            var response = esClient.Delete<T>(pEntity, p => p.Index(pEntity.IndexAlias));
            if (response.Result == Result.Error)
            {
                throw new Exception(response.OriginalException.Message);
            }
            if (pRefresh)
            {
                esClient.GetIndex<T>().Refresh(Elasticsearch.Net.Refresh.True);
            }
        }
        #endregion

        #region Mass Indexing Actions
        /// <summary>
        /// Insert or updates a list of entities into the index
        /// </summary>
        /// <param name="entities">The entities that will be inserted ot update</param>
        /// <param name="idField">The Id field expression</param>
        /// <param name="refresh">Flag to refresh index state with new data</param>
        public static void UpsertEntities<T>(IList<T> entities, Expression<Func<T, object>> idField, bool refresh = false) where T : EsDocument
        {
            var indexname = "";
            if (entities.Count > 0)
            {
                entities[0].CreateIndex<T>();
                indexname = entities[0].IndexAlias;
            }
            else
            {
                return;
            }
            var esClient = Manager.EsClient;
            var descriptor = new BulkDescriptor();
            descriptor.TypeQueryString("_doc");
            foreach (var doc in entities)
            {
                var idVal = Utils.GetObjectValue(idField, doc);
                Id idData = idVal.ToString();

                descriptor.Update<T>(op => op
                .Id(idData)
                .Index(indexname)
                .Doc(doc)
                .DocAsUpsert());
            }
            var response = esClient.Bulk(descriptor);
            if (!response.IsValid)
            {
                throw new Exception(response.OriginalException.Message);
            }
            if (refresh)
            {
                esClient.GetIndex<T>().Refresh(Elasticsearch.Net.Refresh.True);
            }
        }

        /// <summary>
        /// Inserts the passed data into elastic search index
        /// </summary>
        /// <param name="entities">The list of the entities that will be indexed</param>
        public static void BulkInsert<T>(List<T> entities) where T : EsDocument
        {
            if (entities.Count > 0)
            {
                entities[0].CreateIndex<T>();
                var indexName = entities[0].IndexAlias;

                var esClient = Manager.EsClient;

                var descriptor = new BulkDescriptor();
                descriptor.TypeQueryString("_doc");
                foreach (var doc in entities)
                {
                    descriptor.Index<T>(i => i
                        .Index(indexName)
                        .Document(doc));
                }
                var response = esClient.Bulk(descriptor);
                if (!response.IsValid)
                {
                    throw new Exception(response.OriginalException.Message);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// Inserts the passed data into elastic search index Async Mode
        /// </summary>
        /// <param name="entities">The list of the entities that will be indexed</param>
        public static async void BulkAsyncInsert<T>(List<T> entities) where T : EsDocument
        {
            if (entities.Count > 0)
            {
                entities[0].CreateIndex<T>();
                var indexName = entities[0].IndexAlias;
                var esClient = Manager.EsClient;

                var descriptor = new BulkDescriptor();
                foreach (var doc in entities)
                {
                    descriptor.Index<T>(i => i
                        .Index(indexName)
                        .Document(doc));
                }
                var response = await esClient.BulkAsync(descriptor);
                if (!response.IsValid)
                {
                    throw new Exception(response.OriginalException.Message);
                }
            }
            else
            {
                return;
            }
        }
        #endregion

        #region Maintenance

        public static void RefreshIndex<T>() where T : EsDocument, new()
        {
            Manager.EsClient.GetIndex<T>().Refresh(Elasticsearch.Net.Refresh.True);
        }
        /// <summary>
        /// Reindex an index with an updated mapping
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ReIndex<T>(bool removeOldIndex = true, Action<string> errorLogger = null) where T : EsDocument, new()
        {
            var doc = new T();
            doc.ReIndex<T>(removeOldIndex, errorLogger);
        }
        /// <summary>
        /// Deletes index
        /// </summary>
        public static void PurgeIndex<T>() where T : EsDocument, new()
        {
            var indexName = (new T()).IndexAlias;
            Manager.EsClient.Indices.Delete(indexName);
        }

        /// <summary>
        /// CAUTION!!! Deletes all indexes
        /// </summary>
        public static void PurgeIndexes()
        {
            var esClient = Manager.EsClient;
            var indxes = esClient.Cat.Indices();
            foreach (var indx in indxes.Records)
            {
                esClient.Indices.Delete(indx.Index);
            }
        }
        /// <summary>
        /// Backup existing indexes
        /// </summary>
        /// <param name="backUpName">The snapshot and backup repository name</param>
        /// <param name="path">Check elasticsearch.yml for setting this correctly. Path is declared in path.repo["backuppath"] and is located in path.data folder.</param>
        public static void BackUp(string backUpName, string path)
        {
            var esClient = Manager.EsClient;

            var rreq = new CreateRepositoryRequest(backUpName)
            {
                Repository = new FileSystemRepository(new FileSystemRepositorySettings(path))
            };

            var rr = esClient.Snapshot.CreateRepository(rreq);
            if (rr.ServerError != null)
                throw new Exception(rr.ServerError.Error.ToString());
            var sreq = new SnapshotRequest(backUpName, backUpName);
            var sr = esClient.Snapshot.Snapshot(sreq);
            if (sr.ServerError != null)
                throw new Exception(sr.ServerError.Error.ToString());
        }
        /// <summary>
        /// Restore an older index. This action closes all opened indexes
        /// </summary>
        /// <param name="backUpName">The backup name</param>
        public static void Restore(string backUpName)
        {
            var esClient = Manager.EsClient;
            var indxes = esClient.Cat.Indices();
            foreach (var indx in indxes.Records)
            {
                esClient.Indices.Close(indx.Index);
            }
            var r = esClient.Snapshot.Restore(backUpName, backUpName);
            if (r.ServerError != null)
                throw new Exception(r.ServerError.Error.ToString());
        }
        #endregion
    }
}
