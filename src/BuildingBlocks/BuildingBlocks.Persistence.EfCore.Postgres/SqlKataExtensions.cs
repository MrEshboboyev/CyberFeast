using System.Collections.Immutable;
using System.Data;
using Dapper;
using SqlKata;
using SqlKata.Compilers;

namespace BuildingBlocks.Persistence.EfCore.Postgres;

/// <summary>
/// Provides extension methods for <see cref="IDbConnection"/> to use SqlKata with PostgreSQL.
/// </summary>
public static class SqlKataExtensions
{
    private static readonly PostgresCompiler _postgresCompiler = new PostgresCompiler();

    /// <summary>
    /// Asynchronously queries multiple records from the database using SqlKata.
    /// </summary>
    /// <typeparam name="T">The type of the records to query.</typeparam>
    /// <param name="connection">The database connection.</param>
    /// <param name="source">An action to configure the SQL query.</param>
    /// <returns>A read-only list of queried records.</returns>
    public static async Task<IReadOnlyList<T>> QueryAsync<T>(this IDbConnection connection, Action<Query> source)
    {
        var query = new Query();
        source.Invoke(query);

        var compileResult = _postgresCompiler.Compile(query);

        var result = await connection.QueryAsync<T>(compileResult.Sql, compileResult.NamedBindings);

        return result.ToImmutableList();
    }

    /// <summary>
    /// Asynchronously queries a single record from the database using SqlKata.
    /// </summary>
    /// <typeparam name="T">The type of the record to query.</typeparam>
    /// <param name="connection">The database connection.</param>
    /// <returns>The queried record, or null if no record is found.</returns>
    public static Task<T?> QueryOneAsync<T>(this IDbConnection connection)
    {
        var compile = _postgresCompiler.Compile(new Query());
        return connection.QueryFirstOrDefaultAsync<T>(compile.Sql, compile.NamedBindings);
    }
}