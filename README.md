# Marten - document database and event store 

Document databases became popular in last years. They have a lot of places where they are suitable to use. MongoDb, RavenDb are doing very well on production sites. This time I`ll tell something about Marten which has feature of document database and event store. The new thing is that Marten works on the top of Postgres database.

## What is it Marten?

I`ve heard the first time about Marten on Slack channel when some person shared the link to [this post](https://jeremydmiller.com/2016/08/18/moving-from-ravendb-to-marten/). [Jeremy Miller](https://jeremydmiller.com/) shows why his team decided to abandon RavenDb and setup completly new project with utilizing Postgres relational database. Marten just works as an ORM on Postgres and provides functionality of document database and event store. It is great advantage when single relational database can be used in many ways in the project. But as always, if something is for many things then is the best in nothing.

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

### Querying

### Index configuration

### Database session management

Describe types of sessions (light session, session, dirty tracking).

### Foreign key feature


### Limitations

Embedded mode, ORM problems 

## Marten as an event store


### Limitations

## Summary

