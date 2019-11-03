# VB Elastic Fluent

A high level library to connect insert and search Items in elasticsearch nodes with a simplified interface.

## [Nuget](https://www.nuget.org/packages/Vb.Elastic.Fluent)

## Connecting to Elastic Search Service

You connect into an Elastic Search Service using the Manager Instance

The constructor arguments are:

**Your Application Version**: The version of your application, used for upgrading

**Your Application Name**: Your installation/application name name

**Elastic Search Service Uri**: Elasticsearch Uri

	Manager.Instance.Connect("1", "test", "http://localhost:9200");

As an Alternative you can use a Nest **ConnectionSettings** instance to set the connection according to your elasticsearch service.

## Indexing Documents

Indexing documents is handled by **IndexManager** Class
Documents stored with IndexManager need to inherit from **EsDocument** class

To store new data you use the following calls of IndexManager

**IndexEntity** Used to index a document

The parameters are:
**EsDocument Entity**: The entity of the document that will be indexed

**Refresh Index (optional)**: Flag that refresh index state after inserting the document in order the document to be available in next calls (default is false)

	IndexManager.IndexEntity(item, true);

**BulkInsert** and **BulkInsertAsync** Used to store a collection of documents

The parameters are:

**EsDocument Collection**: The collection of document entities that will be indexed

**Refresh Index(optional)**: Flag that refreshes index state after inserting the document in order the document to be available in next calls (default is true)

	IndexManager.BulkInsert(collection, true);

**Note**: A refresh makes all operations performed on an index since the last refresh available for search.

## Update documents

To update the state of the documents you need to use the following call on IndexManager

**UpdateEntity**

The parameters are:

**EsDocument Entity**: The entity of the document that will be indexed

**Refresh Index (optional)**: Flag that refresh index state after inserting the document in order the document to be available in next calls (default is false)

	IndexManager.UpdateEntity(item, true);

## Update or Insert documents Bulk Operation
In order to update or insert a collection of new or existing documents with updated content you need to use the following calls

**UpsertEntities** and **UpsertEntitiesAsync** Used to store a collection of documents

The parameters are:

**EsDocument Collection**: The collection of document entities that will be indexed

**Expression of the Id field**: The expression that defines the id field (eg f=>f.Id)

**Refresh Index(optional)**: Flag that refreshes index state after inserting the document in order the document to be available in next calls (default is false)



	IndexManager.UpsertEntities(item, f=>f.Id, true);

## Store Documents with file Attachments

Elasticsearch support the indexing of files in order to search into their content.
In order to Index a file you need your document object to inherit from **EsAttachment** class

Then you need to use the following calls on IndexManager

## Insert new documents with attachment
For insert you can use the attachment insert calls **IndexAttachment**, **BulkInsertAttachment** and **BulkInsertAttachmentAsync** like the similar calls for document

	IndexManager.IndexAttachment(item, true);
	IndexManager.BulkInsertAttachment(item, true);

## Update documents with attachments
For update you can use the attachment update call **UpdateAttachment** like the documents one

	IndexManager.UpdateAttachment(item, true);

## Update or Insert documents with file attachments Bulk Operation
In order to update or insert a collection of new or existing documents with updated content you need to use the similar to document calls
**UpsertAttachments** and **UpsertAttachmentsAsync**

	IndexManager.UpsertAttachments(item, f=>f.Id, true);

## Delete Document and Attachments
To delete a document or attachment use the following call on **IndexManager**

**DeleteEntity**

The parameters are:

**EsDocument/EsAttachment Entity**: The entity of the document that will be deleted

**Refresh Index (optional)**: Flag that refresh index state after inserting the document in order the document to be available in next calls (default is false)

	IndexManager.DeleteEntity(item, true);

## Search

In order to send a query on elasticsearch service you need to set a **FindRequest** instance

FindRequest constructor arguments are:

**from** (**optional**)  Paging Results Start from default is 0

**max**(**optional**)   Max Items to return default is 20

	var searchRequest = new FindRequest<Item>(0, 10);

### Query clauses

In order to generate query clauses need to use the Class **SearchClause**

**Match**

 Returns documents that match a provided text, number, date or boolean value. The provided text is analyzed before matching. 

Match call arguments:

**field**: The field where we search for matching terms

**query**: Terms query in case you with to search for exact phrase include this phrase between double quotes (**""**)

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Document>.Match(x => x.Content, "clause")

**Term**

Returns documents that contain an **exact** term in a provided field. You can use the `term` query to find documents based on a precise value such as a price, a product ID, or a username.

Term call arguments:

**field**: The field where we search for matching terms

**query**: The search term

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Document>.Term(x => x.Content, "clause")

**Prefix**

Returns documents that contain a specific prefix in a provided field. 

Prefix call arguments:

**field**: The field where we search for matching terms

**query**: The search term

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Document>.Prefix(x => x.Content, "prefix")

**Wildcard**

Returns documents that contain terms matching * wildcard pattern. 

Wildcard call arguments:

**field**: The field where we search for matching terms

**query**: The search term with the wildcard character *****

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Document>.Wildcard(x => x.Content, "ela*c")
**Range**

Returns documents that contain terms within a provided range. 

Range call arguments:

**field**: The field where we search for matching terms

**from**: From this number/date

**to**: To this number/date

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Weight>.Range(x => x.Weight, 119, 201)
**GreaterThan**

Returns documents that are Greater than the provided value. 

GreaterThan call arguments:

**field**: The field where we search for matching terms

**value**: The search term

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Weight>.GreaterThan(x => x.Weight, 119)
**LessThan**

Returns documents that are Less than the provided value. 

LessThan call arguments:

**field**: The field where we search for matching terms

**value**: The search term

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Weight>.LessThan(x => x.Weight, 110)

**Distance**

Filters documents that include only hits that exists within a specific distance from a geo point. 

Distance call arguments:

**field**: The field where we search for matching terms

**latitude**: Latitude of center

**longitude**: Longitude of center

**radius**: Radius of search circle

**nestedField** (**optional**)  : The nested field in case we want to search on a nested field in the field given

**boost** (**optional**)  : Boost used on results

	SearchClause<Place>.Distance(x => x.Location, 38.044631, 23.724513, 500)

### Getting a document

You use the **Get** method to generate a find request to elasticsearch service

Get call arguments:

**clause** : Search clause

**sort** (**optional**) : Sort by Ascending Field

	var results = searchData.Get(SearchClause<Document>.Match(x => x.Content, "my"))

### Occurrences of clauses

You can use the following calls in order to set the occurrence of the query clause, the occurrences are handled by a boolean query container

**Must**

 The clause must appear in matching documents and will contribute to the score.

Must call arguments:

**clause** : Clause Search operation

	var results = searchData.Must(SearchClause<Document>.Match(x => x.Content, "my")).Execute();

**Should**

 The clause should appear in the matching document. 

Should call arguments:

**clause** : Clause Search operation

	var results = searchData.Should(SearchClause<Document>.Match(x => x.Content, "my")).Execute();

**Not**

 The clause must not appear in the matching documents.  

Not call arguments:

**clause** : Clause Search operation

	var results = searchData.Not(SearchClause<Document>.Match(x => x.Content, "my")).Execute();

### Sorting

In order to sort document by a field value you use the sort call

Sort call arguments:

**field**:  The sort field

**sort** (**optional**) : True if direction of sort is Ascending use false for Descending(Default is True)

```
var results = searchData
.Must(SearchClause<Document>.Match(x => x.Content, "my"))
.Sort(x => x.DocDate)
.Execute();
```

If you wish to sort by a GeoPoint you need to use the GeoSort call

GeoSort  call arguments:

**field**:  The sort field

**longitude**: Longitude of center

**radius**: Radius of search circle

**sort** (**optional**) : True if direction of sort is Ascending use false for Descending(Default is True)

    var results = searchData
    	.Should(SearchClause<SampleLocation>.Distance(x => x.Location, 38.044631, 23.724513, 500))
        .GeoSort(x => x.Location, 38.044631, 23.724513)
        .Execute();

### Searching in File Attachment Comment

Attachment File indexed data is stored in the EsAttachment Data attribute. Data is an Attachment data type. The indexed contents of the file are indexed into the Content attribute. 

    var results = searchData
    	.Should(SearchClause<SampleAttachment>.Match(x => x.Data.Content, "text"))
    	.Execute();
### Nested Data Search

Searching for specific attributes in nested data types needs to define the nested field we want to search. 

    var results = searchData
    	.Must(SearchClause<SampleNest>.Range(x => x.Items, new DateTime(2019, 2, 7), new DateTime(2019, 6, 1), x => x.Items[0].Created))
    	.Must(SearchClause<SampleNest>.Range(x => x.Items, 2, 5, x => x.Items[0].Code))
    	.Must(SearchClause<SampleNest>.Term(x => x.Items, "item", x => x.Items[0].Data))
    	.Execute();