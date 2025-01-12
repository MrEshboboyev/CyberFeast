using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.ApiKey.Authorization;

/// <summary>
/// Represents a requirement for users to be in the "Admin" role.
/// </summary>
public class OnlyAdminsRequirement : IAuthorizationRequirement;