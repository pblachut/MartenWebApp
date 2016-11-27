# Marten - document database and event store 

Document databases became popular in last years. They have a lot of places where they are suitable to use. MongoDb, RavenDb are doing very well on production sites. This time I`ll tell something about Marten which has feature of document database and event store. The new thing is that Marten works on the top of Postgres database.

## What is it Marten?

I`ve heard the first time about Marten on Slack channel when some person shared the link to [this post](https://jeremydmiller.com/2016/08/18/moving-from-ravendb-to-marten/). [Jeremy Miller](https://jeremydmiller.com/) shows why his team decided to abandon RavenDb and setup completly new project with utilizing Postgres relational database. Marten just works as an ORM on Postgres and provides functionality of document database and event store. It is great advantage when single relational database can be used in many ways in the project. But as always, if something is for many things then is the best in nothing.

## Marten as a document database

Document database API is very similar to RavenDB what creators of the Marten say openly. They have created Marten as a substitution of RavenDB and that is reason of doing that. 

TODO Tell sth about JSONB https://www.postgresql.org/docs/9.4/static/datatype-json.html

To start using Marten it is needed to create `DocumentStore` object with connection string to Postgres database.

```csharp

var documentStore = DocumentStore.For("connection string");

var documentStore = DocumentStore.For(storeOptions =>
{
    storeOptions.Connection("connection string");
    // the rest of configuration
});

```

Static `DocumentStore` class provides two overrides of `For(..)` method. First one is the simplest one and takes only connection string. Second override is more advanced and gives possibility of additonal configuration using [StoreOptions](https://github.com/JasperFx/marten/blob/master/src/Marten/StoreOptions.cs) class.

By default Marten creates all relational database schemas automatically. It is very useful during development when you don`t want to care about migrations but may be harmful in production environment. 

TODO Command line tool to schema management http://jasperfx.github.io/marten/documentation/cli/

Marten creates separate tables for each document type. Tables are named using given pattern: `mt_doc_documentType` where `documentType` is the name of the document class. It is possible to override this naming by using attribute or explicit declaration in document store configuration.

```csharp

[DocumentAlias("anotheralias)]
public class User
{

}

DocumentStore.For(storeOptions =>
{
    storeOptions.Schema.For<User>().DocumentAlias("anotheralias);
});

``` 

### Database session

It is possible to open modification database session in three different types of mode:
- `_documentStore.LightweightSession()` 

Session with manually document change tracking. To save or update document in the database `_session.Store(document)` must be called. It does not use [IdentityMap](http://jasperfx.github.io/marten/documentation/documents/advanced/identitymap/) mechanism but it must be noted that it would be applided to all documents loaded using `IDocumentSession.Query<T>()` in this session. 


- `_documentStore.OpenSession()`
 
Session with manually document change tracking. It uses IdentityMap by default.

- `_documentStore.DirtyTrackedSession()`

Session with automatically document change tracking. Change in every document loaded to the session would be detected and then `_session.Store(document)` would be invoked automatically. Dirty tracking session is done by comparising JSON documents node by node so enabling it would influence on the performance significantly.

## Transaction isolation level

It is possible to determine transaction isolation level in all above session modes. Default level is `ReadCommitted` but it can be set during opening the session e.g. `_store.DirtyTrackedSession(IsolationLevel.Serializable)`.

## Read-only database session

There is also separate session which was designed only to access database in read-only mode. To create it, it is needed to call `_documentStore.QuerySession()`. Regarding document cache it works the same as in `_documentStore.LightweightSession()`.

### Querying

## Linq

TODO supported operations + raw json queries http://jasperfx.github.io/marten/documentation/documents/querying/query_json/

Fetching multiple documents in single call http://jasperfx.github.io/marten/documentation/documents/querying/include/

## Document Hierarchies http://jasperfx.github.io/marten/documentation/documents/advanced/hierarchies/

## SQL and Postresql SQL queries

## Batch queries http://jasperfx.github.io/marten/documentation/documents/querying/batched_queries/

### Index configuration

### Foreign key feature


### Limitations

Embedded mode, ORM problems, comments made by Ayende

## Marten as an event store


### Limitations

## Summary

