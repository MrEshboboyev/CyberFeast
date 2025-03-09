using Microsoft.AspNetCore.Identity;

namespace FoodDelivery.Services.Identity.Shared.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    #region Properties

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime? LastLoggedInAt { get; set; }
    public DateTime CreatedAt { get; set; }

    #endregion

    #region Navigation

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
    public virtual ICollection<AccessToken> AccessTokens { get; set; } = null!;
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = null!;
    public UserState UserState { get; set; }

    #endregion
}