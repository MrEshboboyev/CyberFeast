using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.ApiKey.Authorization;

/// <summary>
/// Represents a requirement for users to be in the "Third Party" role.
/// </summary>
public class OnlyThirdPartiesRequirement : IAuthorizationRequirement;