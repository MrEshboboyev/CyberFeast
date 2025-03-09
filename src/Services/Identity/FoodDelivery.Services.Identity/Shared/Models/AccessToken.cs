namespace FoodDelivery.Services.Identity.Shared.Models;

public class AccessToken
{
    #region Properties

    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public string CreatedByIp { get; set; } = null!;

    #endregion

    #region Navigation
 
    public ApplicationUser? ApplicationUser { get; set; }

    #endregion
}