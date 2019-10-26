using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using vb.Elastic.Fluent.Objects.Document;

namespace vb.Elastic.Fluent
{
    internal static class Extensions
    {
        internal static string GetIndexByAlias(this ElasticClient client, string aliasName)
        {
            var col = client.GetIndicesPointingToAlias(aliasName);
            var ir = col.FirstOrDefault();
            return ir;
        }
        internal static IndexDescriptor<T> GetIndex<T>(this ElasticClient client) where T : EsDocument
        {
            //var col = client.GetIndicesPointingToAlias(aliasName);
            var indexDesc = new IndexDescriptor<T>(IndexName.From<T>());
            return indexDesc;
        }

    }
}
