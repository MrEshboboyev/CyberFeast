using System.Reflection;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace BuildingBlocks.Core.Paging;

/// <summary>
/// Extends the SieveProcessor to apply custom sorting, filtering, and pagination logic.
/// </summary>
public class ApplicationSieveProcessor : SieveProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationSieveProcessor"/> class with the specified options.
    /// </summary>
    /// <param name="options">The Sieve options.</param>
    public ApplicationSieveProcessor(IOptions<SieveOptions> options)
        : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationSieveProcessor"/> class with the specified options and custom sort methods.
    /// </summary>
    /// <param name="options">The Sieve options.</param>
    /// <param name="customSortMethods">The custom sort methods.</param>
    public ApplicationSieveProcessor(
        IOptions<SieveOptions> options,
        ISieveCustomSortMethods customSortMethods)
        : base(options, customSortMethods)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationSieveProcessor"/> class with the specified options and custom filter methods.
    /// </summary>
    /// <param name="options">The Sieve options.</param>
    /// <param name="customFilterMethods">The custom filter methods.</param>
    public ApplicationSieveProcessor(
        IOptions<SieveOptions> options,
        ISieveCustomFilterMethods customFilterMethods)
        : base(options, customFilterMethods)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationSieveProcessor"/> class with the specified options, custom sort methods, and custom filter methods.
    /// </summary>
    /// <param name="options">The Sieve options.</param>
    /// <param name="customSortMethods">The custom sort methods.</param>
    /// <param name="customFilterMethods">The custom filter methods.</param>
    public ApplicationSieveProcessor(
        IOptions<SieveOptions> options,
        ISieveCustomSortMethods customSortMethods,
        ISieveCustomFilterMethods customFilterMethods)
        : base(options, customSortMethods, customFilterMethods)
    {
    }

    /// <summary>
    /// Maps properties for Sieve processing using configurations from the calling assembly.
    /// </summary>
    /// <param name="mapper">The Sieve property mapper.</param>
    /// <returns>The configured Sieve property mapper.</returns>
    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        return mapper.ApplyConfigurationsFromAssembly(Assembly.GetCallingAssembly());
    }
}