# VB Elastic Fluent

A simple library to connect insert and search Items in elasticsearch nodes.

### Nuget Install
[Nuget](https://www.nuget.org/packages/Vb.Elastic.Fluent)

## Usage Example

### Set up a connection to Elastic Search Service

You connect into an Elastic Search Service using the Manager Instance
The parameters are:
**Your Application Version**: The version of your application, used for upgrading
**Your Application Name**: Your installation/application name name
**Elastic Search Service Uri**: Elasticsearch uri

	Manager.Instance.Connect("1", "test", "http://localhost:9200");

As an Alternative you can use a Nest **ConnectionSettings** instance to set the connection according to your elasticsearch service.

### Indexing

Indexing documents is handled by **IndexManager** Class
Documents stored with IndexManager need to inherit from **EsDocument** class

#### Indexing Documents

##### Indexing new documents
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

##### Update documents

To update the state of the documents you need to use the following call on IndexManager

**UpdateEntity**
The parameters are:
**EsDocument Entity**: The entity of the document that will be indexed
**Refresh Index (optional)**: Flag that refresh index state after inserting the document in order the document to be available in next calls (default is false)

	IndexManager.UpdateEntity(item, true);

##### Update or Insert documents Bulk Operation
In order to update or insert a collection of new or existing documents with updated content you need to use the following calls
**UpsertEntities** and **UpsertEntitiesAsync** Used to store a collection of documents
The parameters are:
**EsDocument Collection**: The collection of document entities that will be indexed
**Expression of the Id field**: The expression that defines the id field (eg f=>f.Id)
**Refresh Index(optional)**: Flag that refreshes index state after inserting the document in order the document to be available in next calls (default is false)

	IndexManager.UpsertEntities(item, f=>f.Id, true);

#### Store Documents with file Attachments

Elasticsearch support the indexing of files in order to search into their content.
In order to Index a file you need your document object to inherit from **EsAttachment** class

Then you need to use the following calls on IndexManager

##### Insert new documents with attachment
For insert you can use the attachment insert calls **IndexAttachment**, **BulkInsertAttachment** and **BulkInsertAttachmentAsync**
like the similar calls for document

	IndexManager.IndexAttachment(item, true);
	IndexManager.BulkInsertAttachment(item, true);

##### Update documents with attachments
For update you can use the attachment update call **UpdateAttachment** like the documents one

	IndexManager.UpdateAttachment(item, true);

##### Update or Insert documents with atttachments Bulk Operation
In order to update or insert a collection of new or existing documents with updated content you need to use the similar to document calls
**UpsertAttachments** and **UpsertAttachmentsAsync**

	IndexManager.UpsertAttachments(item, f=>f.Id, true);

### Delete Document and Attachments
To delete a document or attachment use the following call on IndexManager
**DeleteEntity**
The parameters are:
**EsDocument/EsAttachment Entity**: The entity of the document that will be deleted
**Refresh Index (optional)**: Flag that refresh index state after inserting the document in order the document to be available in next calls (default is false)

	IndexManager.DeleteEntity(item, true);

### Search

#### FindRequest

In order to send a query on elasticsearch service you need to set a FindRequest instance
FindRequest constructor arguments
**from** Paging Results Start from default is 0
**max** Max Items to return default is 20

	var searchRequest = new FindRequest<Item>(0, 10);

##### Query clauses
**Match**
**Term**
**Prefix**
**Wildcard**
**Range**
**GreaterThan**
**LessThan**
**Distance**
##### Occurrences of clauses

You can use the following calls in order to set the occurence of the query clause

**Must**
