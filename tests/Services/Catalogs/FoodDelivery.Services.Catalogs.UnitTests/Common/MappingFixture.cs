using AutoMapper;

namespace FoodDelivery.Services.Catalogs.UnitTests.Common;

public class MappingFixture
{
    public IMapper Mapper { get; } = MapperFactory.Create();
}
