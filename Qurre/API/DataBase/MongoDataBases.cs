using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;
namespace Qurre.API.DataBase
{
    public class MongoDataBase
    {
        internal MongoDataBase(MongoClient _client) => Client = _client;
        internal MongoClient Client { get; private set; }
        public IAsyncCursor<string> ListDatabaseNames(CancellationToken cancellationToken = default) => Client.ListDatabaseNames(cancellationToken);
        public IAsyncCursor<string> ListDatabaseNames(ListDatabaseNamesOptions options, CancellationToken cancellationToken = default) => Client.ListDatabaseNames(options, cancellationToken);
        public IAsyncCursor<string> ListDatabaseNames(IClientSessionHandle session, CancellationToken cancellationToken = default) => Client.ListDatabaseNames(session, cancellationToken);
        public IAsyncCursor<string> ListDatabaseNames(IClientSessionHandle session, ListDatabaseNamesOptions options, CancellationToken cancellationToken = default)
            => Client.ListDatabaseNames(session, options, cancellationToken);
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(CancellationToken cancellationToken = default) => Client.ListDatabaseNamesAsync(cancellationToken);
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(ListDatabaseNamesOptions options, CancellationToken cancellationToken = default)
            => Client.ListDatabaseNamesAsync(options, cancellationToken);
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default)
            => Client.ListDatabaseNamesAsync(session, cancellationToken);
        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(IClientSessionHandle session, ListDatabaseNamesOptions options, CancellationToken cancellationToken = default)
            => Client.ListDatabaseNamesAsync(session, options, cancellationToken);
        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default)
            => Client.ListDatabases(session, options, cancellationToken);
        public IAsyncCursor<BsonDocument> ListDatabases(ListDatabasesOptions options, CancellationToken cancellationToken = default)
            => Client.ListDatabases(options, cancellationToken);
        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, CancellationToken cancellationToken = default)
            => Client.ListDatabases(session, cancellationToken);
        public IAsyncCursor<BsonDocument> ListDatabases(CancellationToken cancellationToken = default) => Client.ListDatabases(cancellationToken);
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(CancellationToken cancellationToken = default) => Client.ListDatabasesAsync(cancellationToken);
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(ListDatabasesOptions options, CancellationToken cancellationToken = default)
            => Client.ListDatabasesAsync(options, cancellationToken);
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default)
            => Client.ListDatabasesAsync(session, cancellationToken);
        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default)
            => Client.ListDatabasesAsync(session, options, cancellationToken);
        public IClientSessionHandle StartSession(ClientSessionOptions options = null, CancellationToken cancellationToken = default) => Client.StartSession(options, cancellationToken);
        public Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default)
            => Client.StartSessionAsync(options, cancellationToken);
        public IChangeStreamCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default) => Client.Watch(pipeline, options, cancellationToken);
        public IChangeStreamCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline,
            ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => Client.Watch(session, pipeline, options, cancellationToken);
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default) => Client.WatchAsync(pipeline, options, cancellationToken);
        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline,
            ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => Client.WatchAsync(session, pipeline, options, cancellationToken);
        public IMongoClient WithReadConcern(ReadConcern readConcern) => Client.WithReadConcern(readConcern);
        public IMongoClient WithReadPreference(ReadPreference readPreference) => Client.WithReadPreference(readPreference);
        public IMongoClient WithWriteConcern(WriteConcern writeConcern) => Client.WithWriteConcern(writeConcern);

    }
}