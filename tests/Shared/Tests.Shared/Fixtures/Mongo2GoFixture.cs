using Mongo2Go;
using Xunit.Sdk;

namespace Tests.Shared.Fixtures;

public class Mongo2GoFixture(IMessageSink messageSink) : IAsyncLifetime
{
    public MongoDbRunner MongoDbRunner { get; set; } = null!;

    public Task InitializeAsync()
    {
        MongoDbRunner = MongoDbRunner.Start();

        return Task.CompletedTask;
    }

    public async Task ResetDbAsync(CancellationToken cancellationToken = default)
    {
        MongoDbRunner.Dispose();
        MongoDbRunner = MongoDbRunner.Start();
        messageSink.OnMessage(
            new DiagnosticMessage($"Mongo fixture started on connection string: {MongoDbRunner.ConnectionString}...")
        );
    }

    public Task DisposeAsync()
    {
        MongoDbRunner.Dispose();
        MongoDbRunner = null!;
        messageSink.OnMessage(new DiagnosticMessage("Mongo fixture stopped."));
        return Task.CompletedTask;
    }
}
