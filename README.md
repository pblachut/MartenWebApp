# Marten - document database and event store 

Document databases became popular in last years. They have a lot of places where they are suitable to use. RavenDb and many others are doing very well on production sites. This time I`ll tell something about Marten which has feature of document database and event store. The new thing is that Marten works on the top of Postgres database.

## What is it Marten?

I`ve heard the first time about Marten on Slack channel when some person shared the link to [this post](https://jeremydmiller.com/2016/08/18/moving-from-ravendb-to-marten/). [Jeremy Miller](https://jeremydmiller.com/) shows why his team decided to abandon RavenDb and setup completly new project with utilizing Postgres relational database. Marten just works as an ORM on Postgres and provides functionality of document database and event store. It is great advantage when single relational database can be used in many ways in the project. But as always, if something is for many things then is the best in nothing.

Marten uses [JSONB](https://www.postgresql.org/docs/9.4/static/datatype-json.html) Postgres type to store documents as default. It gives huge efficiency profits in comparision to normal JSON format. JSONB supports [GIN](https://www.postgresql.org/docs/9.1/static/textsearch-indexes.html) indexing with the following [operators](https://www.postgresql.org/docs/9.4/static/functions-json.html#FUNCTIONS-JSONB-OP-TABLE). Postgres gives also possibility to use [full-text search](https://blog.lateral.io/2015/05/full-text-search-in-milliseconds-with-postgresql/) within JSON documents.

## Marten as a document database

Document database API is very similar to RavenDB what creators of the Marten say openly. They have created Marten as a substitution of RavenDB and that is reason of doing that. 

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

Marten provides a [tool](http://jasperfx.github.io/marten/documentation/cli/) for managing database scheme. It is a nuget package available to use to apply, verify or generate current configuration of Marten documents into SQL database. It also provides functionality of generating up / down SQL scripts to modify database which can be used in any SQL migrations tool.

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

### Session listeners

It is possible to add custom session listeners which can intercept specified action during session unit of work. Such listener must implement `IDocumentSessionListener` interface and must be added during `DocumentStore` configuration. `DocumentSessionListenerBase` is a abstract class which can be used to override specified behaviour. It is not need to implement all listener actions.

```csharp
var listener = new CustomListener();

var store = DocumentStore.For(storeOptions =>
{
    storeOptions.Listeners.Add(listener);
});
```

### Transaction isolation level

It is possible to determine transaction isolation level in all above session modes. Default level is `ReadCommitted` but it can be set during opening the session e.g. `_store.DirtyTrackedSession(IsolationLevel.Serializable)`.

### Read-only database session

There is also separate session which was designed only to access database in read-only mode. To create it, it is needed to call `_documentStore.QuerySession()`. Regarding document cache it works the same as in `_documentStore.LightweightSession()`.

### Optimistic concurrency

Marten gives possibility to enable optimistic concurrency feature on specified document type. After enabling it, any document change which would tried to persist is checked firstly if any other change has been done since document was loaded into cache. Such change is detected by comparing version number located in metadata. It is also possible to store document with manually specified version. More information about it can be found in [documentation](http://jasperfx.github.io/marten/documentation/documents/advanced/optimistic_concurrency/).

## Querying

### Linq

Marten supports [synchronous](http://jasperfx.github.io/marten/documentation/documents/querying/linq/) and [asynchronous](http://jasperfx.github.io/marten/documentation/documents/querying/async/) linq queries quite well. It includes e.g. searching in child collections, deep queries, distinct, compiled queries, ordering, paging, select many and document value projections. Interesting thing regards paging (using `Take()`, `Skip()`) is that it is possible to get total count of all records in single query using [Stats()](http://jasperfx.github.io/marten/documentation/documents/querying/paging/) extension. I think that this functionality was influenced by RavenDb which has similar [one](https://ravendb.net/docs/article-page/3.5/csharp/client-api/session/querying/how-to-get-query-statistics).

```csharp
TODO example with stats
```

As other ORMs, Marten gives possibility to use Postgres SQL in queries, including queries with parameters.

```csharp
var user = session
                .Query<User>("where data ->> 'FirstName' = :FirstName and data ->> 'LastName' = :LastName", 
                            new { FirstName = "Jeremy", LastName = "Miller"})
                .Single();
```

Marten gives possibility to define [foreign keys](http://jasperfx.github.io/marten/documentation/documents/customizing/foreign_keys/) on documents and based on that has possibility to get multiple documents in single query. `Include()` linq extension uses SQL join under the hood to achieve that. As default it uses inner join but it is possible to change it explicitly.

```csharp
TODO example with Include
```

#### Batch queries

It is possible to define set of queries which would be executed in single database call. There are similar soultions in other ORMs which behave like this but many of them work in implicit way (like nHibernate `ToFuture(..)`). Marten do more less the same but gives explicit way of defining such queries. First it is needed to get instance of `IBatchedQuery` from the session and then define the queries which should be done in it.

TODO verify that example

```csharp
var batch = _session.CreateBatchQuery();

var userPromise = batch.Load<User>(id);

var usersPromise = batch.Query<User>().Where(u => u.Name == "Some name").ToList();

await batch.Execute();

var user = await userPromise;
var users = await usersPromise;

```
All queries defined within the batch will return the type `Task<TResult>`. Important thing is that the result of this task can be get only after the batch has been executed.

#### Document hierarchies

Document hierarchies mechanism gives possibility to define inheritance between documents to store them in separate tables. To achieve that it is needed to define appriopriate scheme definition during initiating `DocumentStore` instance. Martren supports one level hierarchies and multi level hierarchies. Both hierarchies types are being configured in similar way.

TODO verify example + add query example + add comment

```csharp
public class User { }
public class Employee : User {}
public class Administrator : User {}

public interface IVehicle {}
public class Car : IVehicle {}
public class Toyota : Car {}

var documentStore = DocumentStore.For(storeOptions =>
{
    storeOptions.Schema.For<User>()
        .AddSubClass<Employee>()
        .AddSubClass(typeof (Administrator));

    storeOptions.Schema.For<IVehicle>()
        .AddSubClassHierarchy(
            typeof(Car),
            typeof(Toyota)
        )
});

```

## Index configuration



## Limitations

Embedded mode, ORM problems, comments made by Ayende, group by missing, full-text search not implemented yet (https://github.com/JasperFx/marten/issues/39)

## Summary

In my personal opinion Marten looks very promising as an alternative to other document databases but cannot be compared directly with them. E.g. RavenDb was created from scratch to build document database when Marten is just a software layer on relational database. It gives a foothold for teams which are used to use relational databases. I claim that it might be very useful in simple usage scenarios but is not polished enough in more advanced, especially if we think about advanced data quering. In comparison to e.g. RavenDB it is much less prepared for this. It must be remembered that Marten is just ORM and has all advantages and drawbacks which ORMs have. Positive aspect is that community concentrated around Marten (including few company contributions) is quite well organised so it forecasts that project will not die in the nearest future and provided functionalities would be still developed.

