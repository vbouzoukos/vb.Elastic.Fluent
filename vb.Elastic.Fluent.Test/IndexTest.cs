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

        [Fact]
        public void GeoSearch()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleLocation>
            {
                new SampleLocation
                {
                    Description = "alpha",
                    Location = new Nest.GeoLocation(38.044842, 23.728171)
                },
                new SampleLocation
                {
                    Description = "beta",
                    Location = new Nest.GeoLocation(38.045316, 23.725822)
                },
                new SampleLocation
                {
                    Description = "gama",
                    Location = new Nest.GeoLocation(38.044378, 23.723365)
                },
                new SampleLocation
                {
                    Description = "out",
                    Location = new Nest.GeoLocation(37.862905, 22.924569)
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleLocation>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleLocation>.Distance(x => x.Location, 38.044631, 23.724513, 500))
                .GeoSort(x => x.Location, 38.044631, 23.724513)
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Equal(3, actual.Count);
            Assert.Equal(expected[2].Description, actual[0].Description);
            Assert.Equal(expected[1].Description, actual[1].Description);
            Assert.Equal(expected[0].Description, actual[2].Description);
        }
        [Fact]
        public void InRange()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleWeight>
            {
                new SampleWeight
                {//0
                    Weight = 100
                },
                new SampleWeight
                {//1
                    Weight = 110
                },
                new SampleWeight
                {//2
                    Weight = 120
                },
                new SampleWeight
                {//3
                    Weight = 130
                },
                new SampleWeight
                {//4
                    Weight = 200
                },
                new SampleWeight
                {//5
                    Weight = 210
                },
                new SampleWeight
                {//6
                    Weight = 220
                },
                new SampleWeight
                {//7
                    Weight = 230
                },
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleWeight>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleWeight>.Range(x => x.Weight, 119, 201))
                .Sort(x => x.Weight)
                .Execute();
            var actual = results.Documents.ToList();
            //range
            Assert.Equal(3, actual.Count);
            Assert.Equal(expected[2].Weight, actual[0].Weight);
            Assert.Equal(expected[3].Weight, actual[1].Weight);
            Assert.Equal(expected[4].Weight, actual[2].Weight);
        }
        [Fact]
        public void GreaterThan()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleWeight>
            {
                new SampleWeight
                {//0
                    Weight = 100
                },
                new SampleWeight
                {//1
                    Weight = 110
                },
                new SampleWeight
                {//2
                    Weight = 120
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleWeight>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleWeight>.GreaterThan(x => x.Weight, 119))
                .Sort(x => x.Weight)
                .Execute();
            var actual = results.Documents.ToList();
            //range
            Assert.Single(actual);
            Assert.Equal(expected[2].Weight, actual[0].Weight);
        }
        [Fact]
        public void LessThan()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleWeight>
            {
                new SampleWeight
                {//0
                    Weight = 100
                },
                new SampleWeight
                {//1
                    Weight = 110
                },
                new SampleWeight
                {//2
                    Weight = 120
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleWeight>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleWeight>.LessThan(x => x.Weight, 110))
                .Sort(x => x.Weight)
                .Execute();
            var actual = results.Documents.ToList();
            //range
            Assert.Single(actual);
            Assert.Equal(expected[0].Weight, actual[0].Weight);
        }
        [Fact]
        public void Nested()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleNest>
            {
                new SampleNest
                {
                    Id="1",
                    Title = "Nested Test",
                    Items=new List<NestedEntity>
                    {
                        new NestedEntity
                        {
                            Created=new DateTime(2019,3,7),
                            Code = 3,
                            Data = "item"
                        }
                    }
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleNest>(0, 10);
            var results = searchData
                .And(SearchTerm<SampleNest>.Range(x => x.Items, new DateTime(2019, 2, 7), new DateTime(2019, 6, 1), x => x.Items[0].Created))
                .And(SearchTerm<SampleNest>.Range(x => x.Items, 2, 5, x => x.Items[0].Code))
                .And(SearchTerm<SampleNest>.Term(x => x.Items, "item", x => x.Items[0].Data))
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Equal(expected.Count, actual.Count);
        }
        [Fact]
        public void DateRange()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleDate>
            {
                new SampleDate
                {//0
                    DocDate=new DateTime(2019,1,1)
                },
                new SampleDate
                {//1
                    DocDate=new DateTime(2019,2,1)
                },
                new SampleDate
                {//2
                    DocDate=new DateTime(2019,3,7)
                },
                new SampleDate
                {//3
                    DocDate=new DateTime(2019,4,7)
                },
                new SampleDate
                {//4
                    DocDate=new DateTime(2019,5,7)
                },
                new SampleDate
                {//5
                    DocDate=new DateTime(2019,6,7)
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleDate>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleDate>.Range(x => x.DocDate, new DateTime(2019, 2, 7), new DateTime(2019, 6, 1)))
                .Sort(x => x.DocDate)
                .Execute();
            var actual = results.Documents.ToList();
            //range
            Assert.Equal(3, actual.Count);
            Assert.Equal(expected[2].DocDate, actual[0].DocDate);
            Assert.Equal(expected[3].DocDate, actual[1].DocDate);
            Assert.Equal(expected[4].DocDate, actual[2].DocDate);
        }
        [Fact]
        public void Future()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleDate>
            {
                new SampleDate
                {//0
                    DocDate=new DateTime(2019,1,1)
                },
                new SampleDate
                {//1
                    DocDate=new DateTime(2019,2,1)
                },
                new SampleDate
                {//2
                    DocDate=new DateTime(2019,3,7)
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleDate>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleDate>.GreaterThan(x => x.DocDate, new DateTime(2019, 2, 7)))
                .Sort(x => x.DocDate)
                .Execute();
            var actual = results.Documents.ToList();
            //range
            Assert.Single(actual);
            Assert.Equal(expected[2].DocDate, actual[0].DocDate);
        }
        [Fact]
        public void Past()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleDate>
            {
                new SampleDate
                {//0
                    DocDate=new DateTime(2019,1,1)
                },
                new SampleDate
                {//1
                    DocDate=new DateTime(2019,2,1)
                },
                new SampleDate
                {//2
                    DocDate=new DateTime(2019,3,7)
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleDate>(0, 10);
            var results = searchData
                .Or(SearchTerm<SampleDate>.LessThan(x => x.DocDate, new DateTime(2019, 1, 7)))
                .Sort(x => x.DocDate)
                .Execute();
            var actual = results.Documents.ToList();
            //range
            Assert.Single(actual);
            Assert.Equal(expected[0].DocDate, actual[0].DocDate);
        }
        [Fact]
        public void Term()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleItem>
            {
                new SampleItem
                {//0
                    Id="1",
                    Code ="A",
                    Sequence=1
                },
                new SampleItem
                {//1
                    Id="2",
                    Code ="B",
                    Sequence=2
                }
            };
            IndexManager.BulkInsert(expected);
            var searchData = new FindRequest<SampleItem>(0, 10);
            var results = searchData
                .And(SearchTerm<SampleItem>.Term(x => x.Code, "A"))
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Single(actual);
            Assert.Equal(expected[0].Id, actual[0].Id);

            searchData = new FindRequest<SampleItem>(0, 10);
            results = searchData
                .And(SearchTerm<SampleItem>.Term(x => x.Sequence, 2))
                .Execute();
            actual = results.Documents.ToList();
            Assert.Single(actual);
            Assert.Equal(expected[1].Id, actual[0].Id);
        }
        [Fact]
        public void Prefix()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleDocument>
            {
                new SampleDocument
                {
                    Id = "1",
                    Sort = "1",
                    Content = @"This a test",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "2",
                    Sort = "2",
                    Content = @"Run a prefix test thisisasample",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "3",
                    Sort = "3",
                    Content = @"no return test",
                    Title = "Mex",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "4",
                    Sort = "4",
                    Content = @"does not exist",
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
                .And(SearchTerm<SampleDocument>.Prefix(x => x.Content, "this"))
                .Sort(x => x.Sort)
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Equal(2, actual.Count);
            Assert.Equal(expected[0].Id, actual[0].Id);
            Assert.Equal(expected[0].Sort, actual[0].Sort);
            Assert.Equal(expected[0].Title, actual[0].Title);
            Assert.Equal(expected[0].DocDate, actual[0].DocDate);
            Assert.Equal(expected[0].Content, actual[0].Content);
            Assert.Equal(expected[1].Id, actual[1].Id);
            Assert.Equal(expected[1].Sort, actual[1].Sort);
            Assert.Equal(expected[1].Title, actual[1].Title);
            Assert.Equal(expected[1].DocDate, actual[1].DocDate);
            Assert.Equal(expected[1].Content, actual[1].Content);

        }
        [Fact]
        public void InWildCard()
        {
            IndexManager.PurgeIndexes();
            var expected = new List<SampleDocument>
            {
                new SampleDocument
                {
                    Id = "1",
                    Sort = "1",
                    Content = @"This a test",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "2",
                    Sort = "2",
                    Content = @"Run a prefix test thisisasample",
                    Title = "Alpha",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "3",
                    Sort = "3",
                    Content = @"no return test",
                    Title = "Mex",
                    DocDate=new DateTime(2019,11,7)
                },
                new SampleDocument
                {
                    Id = "4",
                    Sort = "4",
                    Content = @"does not exist",
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
                .And(SearchTerm<SampleDocument>.Wildcard(x => x.Content, "th*s"))
                .Sort(x => x.Sort)
                .Execute();
            var actual = results.Documents.ToList();
            Assert.Equal(2, actual.Count);
            Assert.Equal(expected[0].Id, actual[0].Id);
            Assert.Equal(expected[0].Sort, actual[0].Sort);
            Assert.Equal(expected[0].Title, actual[0].Title);
            Assert.Equal(expected[0].DocDate, actual[0].DocDate);
            Assert.Equal(expected[0].Content, actual[0].Content);
            Assert.Equal(expected[1].Id, actual[1].Id);
            Assert.Equal(expected[1].Sort, actual[1].Sort);
            Assert.Equal(expected[1].Title, actual[1].Title);
            Assert.Equal(expected[1].DocDate, actual[1].DocDate);
            Assert.Equal(expected[1].Content, actual[1].Content);
        }
    }
}
