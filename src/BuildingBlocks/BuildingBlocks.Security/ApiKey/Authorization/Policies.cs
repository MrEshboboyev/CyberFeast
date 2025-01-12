namespace BuildingBlocks.Security.ApiKey.Authorization;

/// <summary>
/// Defines constants for different authorization policies.
/// </summary>
public static class Policies
{
    /// <summary>
    /// Policy for customers.
    /// </summary>
    public const string OnlyCustomers = nameof(OnlyCustomers);

    /// <summary>
    /// Policy for administrators.
    /// </summary>
    public const string OnlyAdmins = nameof(OnlyAdmins);

    /// <summary>
    /// Policy for third parties.
    /// </summary>
    public const string OnlyThirdParties = nameof(OnlyThirdParties);
}