using FoodDelivery.Services.Customers.Customers.Dtos.v1;
using FoodDelivery.Services.Customers.Customers.Features.CreatingCustomer.v1.Events.Domain;
using FoodDelivery.Services.Customers.Customers.Features.CreatingCustomer.v1.Read.Mongo;
using FoodDelivery.Services.Customers.Customers.Features.UpdatingCustomer.v1;
using FoodDelivery.Services.Customers.Customers.Features.UpdatingCustomer.v1.Events.Domain;
using FoodDelivery.Services.Customers.Customers.Features.UpdatingCustomer.v1.Read.Mongo;
using FoodDelivery.Services.Customers.Customers.Models;
using FoodDelivery.Services.Customers.Customers.Models.Reads;
using Riok.Mapperly.Abstractions;

namespace FoodDelivery.Services.Customers.Customers;

[Mapper]
internal static partial class CustomersModuleMapping
{
    [MapProperty(nameof(CustomerCreated.Id), nameof(CreateCustomerRead.CustomerId))]
    [MapProperty(nameof(CustomerCreated.CreatedAt), nameof(CreateCustomerRead.Created))]
    [MapProperty(nameof(CustomerCreated.Address), nameof(CreateCustomerRead.DetailAddress))]
    internal static partial CreateCustomerRead ToCreateCustomerRead(this CustomerCreated customerCreated);

    [MapperIgnoreTarget(nameof(CustomerReadModel.Id))]
    [MapProperty(nameof(CreateCustomerRead.CustomerId), nameof(CustomerReadModel.CustomerId))]
    internal static partial CustomerReadModel ToCustomer(this CreateCustomerRead createCustomerRead);

    [MapProperty(nameof(CustomerReadModel.FullName), nameof(CustomerReadDto.Name))]
    internal static partial CustomerReadDto ToCustomerReadDto(this CustomerReadModel customer);

    [MapProperty(nameof(Customer.Id), nameof(CreateCustomerRead.CustomerId))]
    [MapProperty(
        $"{nameof(Customer.Address)}.{nameof(Customer.Address.Country)}",
        nameof(CreateCustomerRead.Country)
    )]
    [MapProperty(
        $"{nameof(Customer.Address)}.{nameof(Customer.Address.City)}",
        nameof(CreateCustomerRead.City)
    )]
    [MapProperty(
        $"{nameof(Customer.Address)}.{nameof(Customer.Address.Detail)}",
        nameof(CreateCustomerRead.DetailAddress)
    )]
    [MapProperty(
        $"{nameof(Customer.Nationality)}.{nameof(Customer.Nationality.Value)}",
        nameof(CreateCustomerRead.Nationality)
    )]
    [MapProperty(
        $"{nameof(Customer.Email)}.{nameof(Customer.Email.Value)}",
        nameof(CreateCustomerRead.Email)
    )]
    [MapProperty(
        $"{nameof(Customer.BirthDate)}.{nameof(Customer.BirthDate.Value)}",
        nameof(CreateCustomerRead.BirthDate)
    )]
    [MapProperty(
        $"{nameof(Customer.PhoneNumber)}.{nameof(Customer.PhoneNumber.Value)}",
        nameof(CreateCustomerRead.PhoneNumber)
    )]
    [MapProperty(
        $"{nameof(Customer.Name)}.{nameof(Customer.Name.FirstName)}",
        nameof(CreateCustomerRead.FirstName)
    )]
    [MapProperty(
        $"{nameof(Customer.Name)}.{nameof(Customer.Name.LastName)}",
        nameof(CreateCustomerRead.LastName)
    )]
    [MapProperty(
        $"{nameof(Customer.Name)} . {nameof(Customer.Name.FullName)}",
        nameof(CreateCustomerRead.FullName)
    )]
    internal static partial CreateCustomerRead ToCreateCustomerRead(this Models.Customer customer);

    [MapProperty(nameof(Customer.Id), nameof(UpdateCustomerRead.CustomerId))]
    [MapProperty(
        $"{nameof(Customer.Address)}.{nameof(Customer.Address.City)}",
        nameof(UpdateCustomerRead.Country)
    )]
    [MapProperty(
        $"{nameof(Customer.Address)} . {nameof(Customer.Address.City)}",
        nameof(UpdateCustomerRead.City)
    )]
    [MapProperty(
        $"{nameof(Customer.Address)} . {nameof(Customer.Address.Detail)}",
        nameof(UpdateCustomerRead.DetailAddress)
    )]
    [MapProperty(
        $"{nameof(Customer.Nationality)} . {nameof(Customer.Nationality.Value)}",
        nameof(UpdateCustomerRead.Nationality)
    )]
    [MapProperty(
        $"{nameof(Customer.Email)} . {nameof(Customer.Email.Value)}",
        nameof(UpdateCustomerRead.Email)
    )]
    [MapProperty(
        $"{nameof(Customer.BirthDate)} . {nameof(Customer.BirthDate.Value)}",
        nameof(UpdateCustomerRead.BirthDate)
    )]
    [MapProperty(
        $"{nameof(Customer.PhoneNumber)} . {nameof(Customer.PhoneNumber.Value)}",
        nameof(UpdateCustomerRead.PhoneNumber)
    )]
    [MapProperty(
        $"{nameof(Customer.Name)} . {nameof(Customer.Name.FirstName)}",
        nameof(UpdateCustomerRead.FirstName)
    )]
    [MapProperty(
        $"{nameof(Customer.Name)} . {nameof(Customer.Name.LastName)}",
        nameof(UpdateCustomerRead.LastName)
    )]
    [MapProperty(
        $"{nameof(Customer.Name)} . {nameof(Customer.Name.FullName)}",
        nameof(UpdateCustomerRead.FullName)
    )]
    internal static partial UpdateCustomerRead ToUpdateCustomerRead(this Customer customer);

    // Todo: doesn't map correctly
    internal static CustomerReadModel ToCustomer(this UpdateCustomerRead updateCustomerRead)
    {
        return new Customer
        {
            Created = updateCustomerRead.OccurredOn,
            Email = updateCustomerRead.Email,
            CustomerId = updateCustomerRead.CustomerId,
            IdentityId = updateCustomerRead.IdentityId,
            FirstName = updateCustomerRead.FirstName,
            LastName = updateCustomerRead.LastName,
            FullName = updateCustomerRead.FullName,
            PhoneNumber = updateCustomerRead.PhoneNumber,
            Country = updateCustomerRead.Country,
            City = updateCustomerRead.City,
            DetailAddress = updateCustomerRead.DetailAddress,
            Nationality = updateCustomerRead.Nationality,
            BirthDate = updateCustomerRead.BirthDate,
            Id = updateCustomerRead.Id
        };
    }

    [MapperIgnoreTarget(nameof(UpdateCustomerRead.Id))]
    [MapProperty(nameof(CustomerUpdated.Id), nameof(UpdateCustomerRead.CustomerId))]
    [MapProperty(nameof(CustomerUpdated.IdentityId), nameof(UpdateCustomerRead.IdentityId))]
    internal static partial UpdateCustomerRead ToUpdateCustomerRead(this CustomerUpdated customerUpdated);

    [MapPropertyFromSource(nameof(UpdateCustomer.Id), Use = nameof(MapUpdateCustomerDefaultId))]
    [MapProperty(nameof(UpdateCustomerRequest.Address), nameof(UpdateCustomer.DetailAddress))]
    [MapProperty(nameof(UpdateCustomerRequest.BirthDate), nameof(UpdateCustomer.BirthDate))]
    [MapProperty(nameof(UpdateCustomerRequest.PhoneNumber), nameof(UpdateCustomer.PhoneNumber))]
    [MapProperty(nameof(UpdateCustomerRequest.Email), nameof(UpdateCustomer.Email))]
    internal static partial UpdateCustomer ToUpdateCustomer(this UpdateCustomerRequest updateCustomerRequest);

    private static long MapUpdateCustomerDefaultId(UpdateCustomerRequest request) => 0;

    internal static partial IQueryable<CustomerReadDto> ProjectToCustomerReadDto(
        this IQueryable<CustomerReadModel> queryable
    );
}
