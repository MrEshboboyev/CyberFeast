using Riok.Mapperly.Abstractions;

[assembly: MapperDefaults(
    EnumMappingIgnoreCase = true, 
    EnumMappingStrategy = EnumMappingStrategy.ByName)]

namespace FoodDelivery.Services.Customers;

public class CustomersMetadata;