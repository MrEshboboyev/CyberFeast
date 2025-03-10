using FoodDelivery.Services.Customers.RestockSubscriptions.Dtos.v1;
using FoodDelivery.Services.Customers.RestockSubscriptions.Features.CreatingRestockSubscription.v1;
using FoodDelivery.Services.Customers.RestockSubscriptions.Models.Read;
using Riok.Mapperly.Abstractions;

namespace FoodDelivery.Services.Customers.RestockSubscriptions;

[Mapper]
internal static partial class RestockSubscriptionsModuleMapping
{
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.Id)}.{nameof(Models.Write.RestockSubscription.Id.Value)}",
        nameof(RestockSubscriptionDto.Id)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.Email)}.{nameof(Models.Write.RestockSubscription.Email.Value)}",
        nameof(RestockSubscriptionDto.Email)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.ProductInformation)}.{nameof(Models.Write.RestockSubscription.ProductInformation.Name)}",
        nameof(RestockSubscriptionDto.ProductName)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.ProductInformation)}.{nameof(Models.Write.RestockSubscription.ProductInformation.Id.Value)}",
        nameof(RestockSubscriptionDto.ProductId)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.CustomerId)}.{nameof(Models.Write.RestockSubscription.CustomerId.Value)}",
        nameof(RestockSubscriptionDto.CustomerId)
    )]
    internal static partial RestockSubscriptionDto ToRestockSubscriptionDto(
        this Models.Write.RestockSubscription restockSubscription
    );

    [MapperIgnoreTarget(nameof(RestockSubscriptionReadModel.Id))]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.Id)}.{nameof(Models.Write.RestockSubscription.Id.Value)}",
        nameof(RestockSubscriptionReadModel.RestockSubscriptionId)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.Email)}.{nameof(Models.Write.RestockSubscription.Email.Value)}",
        nameof(RestockSubscriptionReadModel.Email)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.ProductInformation)}.{nameof(Models.Write.RestockSubscription.ProductInformation.Name)}",
        nameof(RestockSubscriptionReadModel.ProductName)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.ProductInformation)}.{nameof(Models.Write.RestockSubscription.ProductInformation.Id.Value)}",
        nameof(RestockSubscriptionReadModel.ProductId)
    )]
    [MapProperty(
        $"{nameof(Models.Write.RestockSubscription.CustomerId)}.{nameof(Models.Write.RestockSubscription.CustomerId.Value)}",
        nameof(RestockSubscriptionReadModel.CustomerId)
    )]
    internal static partial RestockSubscriptionReadModel ToRestockSubscription(
        this Models.Write.RestockSubscription restockSubscription
    );

    [MapProperty(nameof(RestockSubscriptionReadModel.RestockSubscriptionId), nameof(RestockSubscriptionDto.Id))]
    internal static partial RestockSubscriptionDto ToRestockSubscriptionDto(
        this RestockSubscriptionReadModel restockSubscription
    );

    [MapperIgnoreTarget(nameof(RestockSubscriptionReadModel.Id))]
    [MapProperty(
        nameof(CreateMongoRestockSubscriptionReadModels.RestockSubscriptionId),
        nameof(RestockSubscriptionReadModel.RestockSubscriptionId)
    )]
    internal static partial RestockSubscriptionReadModel ToRestockSubscription(
        this CreateMongoRestockSubscriptionReadModels createMongoRestockSubscriptionReadModels
    );

    internal static partial IQueryable<RestockSubscriptionDto> ProjectToRestockSubscriptionDto(
        this IQueryable<RestockSubscriptionReadModel> queryable
    );
}
