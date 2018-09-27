using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using vb.Elastic.Fluent.Core;
using vb.Elastic.Fluent.Indexer;
using vb.Elastic.Fluent.Search;

namespace vb.Elastic.Fluent.Test
{
    [TestClass]
    public class IndexTest
    {
        public IndexTest()
        {
            Manager.Instance.Connect("1", "http://localhost:9200", "test");
        }
        [TestMethod]
        public void Indexing()
        {
            IndexManager.PurgeIndexes();
            var expected = new SampleDocument
            {
                Id = Guid.NewGuid().ToString(),
                Content = @"When be draw drew ye. Defective in do recommend suffering. House it seven in spoil tiled court. Sister others marked fat missed did out use. Alteration possession dispatched collecting instrument travelling he or on. Snug give made at spot or late that mr. ",
                Title = "Defective recommend"
            };
            IndexManager.IndexEntity(expected, true);
            var searchData = new FindRequest<SampleDocument>(0, 10);
            var results = searchData
                .And(SearchTerm<SampleDocument>.Term(x=>x.Id, expected.Id))
                .Execute();
            var actual = results.Documents.FirstOrDefault();
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Content, actual.Content);
            Assert.AreEqual(expected.Title, actual.Title);
        }
        [TestMethod]
        public void Search()
        {
        }
    }
}
