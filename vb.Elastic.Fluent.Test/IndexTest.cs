using System;
using vb.Elastic.Fluent.Core;
using Xunit;
using System.Linq;
using vb.Elastic.Fluent.Indexer;
using vb.Elastic.Fluent.Search;
using System.Collections.Generic;

namespace vb.Elastic.Fluent.Test
{
    public class IndexTest
    {
        public IndexTest()
        {
            Manager.Instance.Connect("1", "http://localhost:9200", "test");
        }
        [Fact]
        public void Indexing()
        {
            IndexManager.PurgeIndexes();
            var expected = new SampleDocument
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Content = @"When be draw drew ye. Defective in do recommend suffering. House it seven in spoil tiled court. Sister others marked fat missed did out use. Alteration possession dispatched collecting instrument travelling he or on. Snug give made at spot or late that mr. ",
                Title = "Defective recommend"
            };
            IndexManager.IndexEntity(expected, true);
            var searchData = new FindRequest<SampleDocument>(0, 10);
            var results = searchData
                .And(SearchTerm<SampleDocument>.Term(x => x.Id, expected.Id))
                .Execute();
            var actual = results.Documents.FirstOrDefault();
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Content, actual.Content);
            Assert.Equal(expected.Title, actual.Title);
        }
        [Fact]
        public void Search()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleDocument>
            {
                new SampleDocument
                {
                    Id = "1",
                    Sort = "1",
                    Content = @"Omega",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "2",
                    Sort = "2",
                    Content = @"delta",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "3",
                    Sort = "3",
                    Content = @"thisfind",
                    Title = "Mex",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "4",
                    Sort = "4",
                    Content = @"trit",
                    Title = "pep",
                    DocDate=new DateTime(2019,11,1)
                },
                new SampleDocument
                {
                    Id = "5",
                    Sort = "5",
                    Content = @"date",
                    Title = "date",
                    DocDate=new DateTime(2019,11,7)
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleDocument>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleDocument>.Term(x => x.Content, "thisfind"))
                .Or(SearchTerm<SampleDocument>.Term(x => x.Title, "alpha"))
                .Or(SearchTerm<SampleDocument>.DateFuture(x => x.DocDate, new DateTime(2019, 11, 5)))
                .Sort(x => x.Sort)
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Equal(4, actual.Count);
            Assert.Equal("1", actual[0].Id);
            Assert.Equal("2", actual[1].Id);
            Assert.Equal("3", actual[2].Id);
            Assert.Equal("5", actual[3].Id);
        }

        [Fact]
        public void ReIndexing()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleDocument>
            {
                new SampleDocument
                {
                    Id = "1",
                    Sort = "1",
                    Content = @"Omega",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "2",
                    Sort = "2",
                    Content = @"delta",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "3",
                    Sort = "3",
                    Content = @"thisfind",
                    Title = "Mex",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "4",
                    Sort = "4",
                    Content = @"trit",
                    Title = "pep",
                    DocDate=new DateTime(2019,11,1)
                },
                new SampleDocument
                {
                    Id = "5",
                    Sort = "5",
                    Content = @"date",
                    Title = "date",
                    DocDate=new DateTime(2019,11,7)
                }
            };
            IndexManager.BulkInsert(expected);
            Manager.Instance.AppVersion = "2";
            IndexManager.ReIndex<SampleDocument>();
            var searchData = new FindRequest<SampleDocument>(0, 10);
            var results = searchData
                .Sort(x => x.Sort)
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].Sort, actual[i].Sort);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].DocDate, actual[i].DocDate);
                Assert.Equal(expected[i].Content, actual[i].Content);
            }
        }
        [Fact]
        public void InsertAttachment()
        {
            var bytes = System.IO.File.ReadAllBytes("Attachment.txt");
            var file = Convert.ToBase64String(bytes);
            IndexManager.PurgeIndexes();
            var expected = new List<SampleAttachment>
            {
                new SampleAttachment
                {
                    Id = "1",
                    Content = file
                }
            };
            IndexManager.IndexAttachment(expected[0], true);
            var searchData = new FindRequest<SampleAttachment>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleAttachment>.Match(x => x.Data.Content, "text"))
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Equal(expected.Count, actual.Count);
        }
    }
}
