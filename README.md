# Marten - document database and event store 

Document databases became popular in last years. They have a lot of places where they are suitable to use. RavenDb and many others are doing very well on production sites. This time I`ll tell something about Marten which has feature of document database and event store. The new thing is that Marten works on the top of Postgres database.
===

#### What is it Marten?

I`ve heard the first time about Marten on Slack channel when some person shared the link to [this post](https://jeremydmiller.com/2016/08/18/moving-from-ravendb-to-marten/). [Jeremy Miller](https://jeremydmiller.com/) shows why his team decided to abandon RavenDb and setup completly new project with utilizing Postgres relational database. Marten just works similar to ORM on Postgres and provides functionality of document database and event store. It is great advantage when single relational database can be used in many ways in the project. But as always, if something is for many things then is the best in nothing.

Marten uses [JSONB](https://www.postgresql.org/docs/9.4/static/datatype-json.html) Postgres type to store documents as default. It gives huge efficiency profits in comparision to normal JSON format. JSONB supports [GIN](https://www.postgresql.org/docs/9.1/static/textsearch-indexes.html) indexing with the following [operators](https://www.postgresql.org/docs/9.4/static/functions-json.html#FUNCTIONS-JSONB-OP-TABLE). Postgres gives also possibility to use [full-text search](https://blog.lateral.io/2015/05/full-text-search-in-milliseconds-with-postgresql/) within JSON documents.

#### Marten as a document database

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

Some people may disagree with the sentence that Marten works like ORM but in my opinion it does. Each object type is stored in separate table and each row in the table represents specified object instance. Marten maps the JSON field inside the row into instance of some type. Like  in other ORMs it is possible to ovveride default mapping convention. In Marten it is possible by using attributes on class or fluent API.

```csharp

[DocumentAlias("anotheralias)]
public class User
{

}

DocumentStore.For(storeOptions =>
{
    storeOptions.Schema.For<User>().DocumentAlias("anotheralias");
});

``` 

#### Database session

It is possible to open modification database session in three different types of mode:
- `_documentStore.LightweightSession()` 

Session with manually document change tracking. To save or update document in the database `_session.Store(document)` must be called. It does not use [IdentityMap](http://jasperfx.github.io/marten/documentation/documents/advanced/identitymap/) mechanism but it must be noted that it would be applided to all documents loaded using `IDocumentSession.Query<T>()` in this session. 


- `_documentStore.OpenSession()`
 
Session with manually document change tracking. It uses IdentityMap by default.

- `_documentStore.DirtyTrackedSession()`

Session with automatically document change tracking. Change in every document loaded to the session would be detected and then `_session.Store(document)` would be invoked automatically. Dirty tracking session is done by comparising JSON documents node by node so enabling it would influence on the performance significantly.

#### Session listeners

It is possible to add custom session listeners which can intercept specified action during session unit of work. Such listener must implement `IDocumentSessionListener` interface and must be added during `DocumentStore` configuration. `DocumentSessionListenerBase` is a abstract class which can be used to override specified behaviour. It is not need to implement all listener actions.

```csharp
var listener = new CustomListener();

var store = DocumentStore.For(storeOptions =>
{
    storeOptions.Listeners.Add(listener);
});
```

#### Transaction isolation level

It is possible to determine transaction isolation level in all above session modes. Default level is `ReadCommitted` but it can be set during opening the session e.g. `_store.DirtyTrackedSession(IsolationLevel.Serializable)`.

#### Read-only database session

There is also separate session which was designed only to access database in read-only mode. To create it, it is needed to call `_documentStore.QuerySession()`. Regarding document cache it works the same as in `_documentStore.LightweightSession()`.

#### Optimistic concurrency

Marten gives possibility to enable optimistic concurrency feature on specified document type. After enabling it, any document change which would tried to persist is checked firstly if any other change has been done since document was loaded into cache. Such change is detected by comparing version number located in metadata. It is also possible to store document with manually specified version. More information about it can be found in [documentation](http://jasperfx.github.io/marten/documentation/documents/advanced/optimistic_concurrency/).

#### Querying

Marten supports [synchronous](http://jasperfx.github.io/marten/documentation/documents/querying/linq/) and [asynchronous](http://jasperfx.github.io/marten/documentation/documents/querying/async/) linq queries quite well. It includes e.g. searching in child collections, deep queries, distinct, compiled queries, ordering, paging, select many and document value projections. Interesting thing regards paging (using `Take()`, `Skip()`) is that it is possible to get total count of all records in single query using [Stats()](http://jasperfx.github.io/marten/documentation/documents/querying/paging/) extension. I think that this functionality was influenced by RavenDb which has similar [one](https://ravendb.net/docs/article-page/3.5/csharp/client-api/session/querying/how-to-get-query-statistics).

```csharp
QueryStatistics statistics;

var result = await _session.Query<User>()
                           .Stats(out statistics)
                           .ToListAsync();

var totalCount = statistics.TotalResults;
```

As other ORMs, Marten gives possibility to use Postgres SQL in queries, including queries with parameters.

```csharp
var user = session
                .Query<User>("where data ->> 'FirstName' = :FirstName and data ->> 'LastName' = :LastName", 
                            new { FirstName = "Jeremy", LastName = "Miller"})
                .Single();
```

Marten has possibility to get multiple documents in single query. `Include()` linq extension uses SQL join under the hood to achieve that. As default it uses inner join but it is possible to change it explicitly. `Include()` construction works in similar way as in RavenDb. It is possible to get single document or documents list.

```csharp
using (var session = _documentStore.LightweightSession())
{
    Company company = null;

    var result = await session.Query<User>()
        .Include<Company>(user => user.CompanyId2, comp => company = comp)
        .SingleAsync(user => user.Id == id);

    return new UserWithCompany
    {
        Company = company,
        User = result
    };
}
```

#### Batch queries

It is possible to define set of queries which would be executed in single database call. There are similar soultions in other ORMs which behave like this but many of them work in implicit way (like nHibernate `ToFuture(..)`). Marten do more less the same but gives explicit way of defining such queries. First it is needed to get instance of `IBatchedQuery` from the session and then define the queries which should be done in it.

```csharp
using (var session = _documentStore.LightweightSession())
{
    var batch = session.CreateBatchQuery();

    var userPromise = batch.Load<User>(id);

    var usersPromise = batch.Query<User>().Where(u => u.FirstName.StartsWith("Name")).ToList();

    await batch.Execute();

    var user = await userPromise;
    var users = await usersPromise;

    return new BatchUsers
    {
        User = user,
        Users = users.ToList()
    };
}

```
All queries defined within the batch will return the type `Task<TResult>`. Important thing is that the result of this task can be get only after the batch has been executed.

#### Document hierarchies

Document hierarchies mechanism gives possibility to define inheritance between documents to query them separately. To achieve that it is needed to define appriopriate scheme definition during initiating `DocumentStore` instance. Martren supports one level hierarchies and multi level hierarchies. Both hierarchies types are being configured in similar way.


```csharp
// one level
public class GeneralUser { }
public class Employee : GeneralUser {}
public class Administrator : GeneralUser {}

// multi level
public interface IVehicle {}
public class Car : IVehicle {}
public class Toyota : Car {}

var documentStore = DocumentStore.For(storeOptions =>
{
    storeOptions.Schema.For<GeneralUser>()
        .AddSubClass<Employee>()
        .AddSubClass(typeof (Administrator));

    storeOptions.Schema.For<IVehicle>()
        .AddSubClassHierarchy(
            typeof(Car),
            typeof(Toyota)
        )
});

```

Table is created for the most general type. It has additional columns named `mt_doc_type` and `mt_dotnet_doc_type` to select appriopriate rows in the query and to deserialize received JSON into correct type. 

Quering the `IVehicle` type results in SQL which is the same as for types without hierarchies. The differencies are during performing a query for `Car` or `Toyota` types.

```sql
select d.data, d.id, d.mt_doc_type, d.mt_version 
from public.mt_doc_ivehicle as d 
where d.mt_doc_type = 'toyota' or d.mt_doc_type = 'car'

select d.data, d.id, d.mt_doc_type, d.mt_version 
from public.mt_doc_ivehicle as d 
where d.mt_doc_type = 'toyota'

```

#### Summary

In my personal opinion Marten looks very promising as an alternative to other document databases but cannot be compared directly with them. E.g. RavenDb was created from scratch to build document database when Marten is just a software layer on relational database. Marten gives a foothold for teams which are used to use relational databases. On the other hand, if we would like to see Marten advantages over RavenDb, Marten is fully [ACID](https://en.wikipedia.org/wiki/ACID). RavenDb supports it only in some [queries](https://ayende.com/blog/164066/ravendb-acid-base).

I claim that Marten might be very useful in simple usage scenarios but is not polished enough in more advanced cases, especially if we think about advanced data quering. 
Full-text search is doable in Postrgres but it is not supported [yet](https://github.com/JasperFx/marten/issues/39). Queries do not allow to perform `GroupBy` operation. Multitenancy is going to be made in `2.0` version but there are still a lot of [discussions](https://github.com/JasperFx/marten/issues/435) and uncertainty how to achieve that. 
Most of advanced features require database scheme changes. There are [tools](http://jasperfx.github.io/marten/documentation/cli/) which enables it but they are not ready to use out of the box. Each project must prepare own adjustments to them. Without good practices, inappriopriate usage may be very harmful especially in production databases. 

It must be remembered that Marten is just ORM and has all advantages and drawbacks which ORMs have. Positive aspect is that community concentrated around Marten (including few company contributions) is quite well organised so it forecasts that project will not die and provided functionalities would be still developed and improved.


Examples of Marten usages (as a document database and as en event store) can be found [here](https://github.com/pblachut/MartenWebApp).
