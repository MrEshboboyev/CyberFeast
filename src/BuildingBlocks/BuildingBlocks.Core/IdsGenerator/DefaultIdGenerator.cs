using Bogus;
using IdGen;
using BuildingBlocks.Abstractions.Core;

namespace BuildingBlocks.Core.IdsGenerator;

/// <summary>
/// Generates unique 64-bit IDs using the Snowflake algorithm.
/// </summary>
public class SnowFlakIdGenerator : BuildingBlocks.Abstractions.Core.IIdGenerator<long>
{
    /// <summary>
    /// Generates a new unique ID.
    /// </summary>
    public long New()
    {
        return NewId();
    }

    /// <summary>
    /// Generates a new unique ID using the Snowflake algorithm.
    /// </summary>
    public static long NewId()
    {
        return _generator.CreateId();
    }

    private static readonly IdGenerator _generator = new(
        new Faker().Random.Number(0, 3), GetOptions());

    /// <summary>
    /// Configures the options for the Snowflake ID generator.
    /// </summary>
    /// <returns>The configured options for the ID generator.</returns>
    public static IdGeneratorOptions GetOptions()
    {
        // Set the epoch to January 17, 2022
        var epoch = new DateTime(
            2024,
            1, 
            9,
            0,
            0,
            0,
            DateTimeKind.Local);

        // Create an ID structure with 45 bits for timestamp, 2 for generator-id, and 16 for sequence
        var structure = new IdStructure(
            45,
            2,
            16);

        // Prepare options
        var options = new IdGeneratorOptions(
            structure,
            new DefaultTimeSource(epoch));

        return options;
    }
}

