using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.ApiKey.Authorization;

/// <summary>
/// Represents a requirement for users to be in the "Customer" role.
/// </summary>
public class OnlyCustomersRequirement : IAuthorizationRequirement;