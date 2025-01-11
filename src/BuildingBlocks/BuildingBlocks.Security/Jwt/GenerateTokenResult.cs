namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Represents the result of a token generation process.
/// </summary>
/// <param name="AccessToken">The generated access token.</param>
/// <param name="ExpireAt">The expiration time of the access token.</param>
public record GenerateTokenResult(string AccessToken, DateTime ExpireAt);