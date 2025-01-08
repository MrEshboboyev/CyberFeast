namespace BuildingBlocks.Core.Persistence.EFCore;

/// <summary>
/// Defines constants used in Entity Framework Core configurations.
/// </summary>
public static class EfConstants
{
    public const string UuidGenerator = "uuid-ossp";
    public const string UuidAlgorithm = "uuid_generate_v4()";
    public const string DateAlgorithm = "now()";

    /// <summary>
    /// Defines various column types.
    /// </summary>
    public static class ColumnTypes
    {
        public const string PriceDecimal = "decimal(18,2)";
        public const string TinyText = "varchar(15)";
        public const string ShortText = "varchar(25)";
        public const string NormalText = "varchar(50)";
        public const string MediumText = "varchar(100)";
        public const string LongText = "varchar(250)";
        public const string ExtraLongText = "varchar(500)";
    }

    /// <summary>
    /// Defines various lengths for text fields.
    /// </summary>
    public static class Length
    {
        public const int Tiny = 15;
        public const int Short = 25;
        public const int Normal = 50;
        public const int Medium = 100;
        public const int Long = 250;
        public const int ExtraLong = 500;
    }
}