using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Core.Domain.ValueObjects;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Validation.Extensions;
using FluentValidation;
using FoodDelivery.Services.Customers.Customers.Exceptions.Application;
using FoodDelivery.Services.Customers.Customers.ValueObjects;
using FoodDelivery.Services.Customers.Shared.Data;
using Microsoft.Extensions.Logging;

namespace FoodDelivery.Services.Customers.Customers.Features.UpdatingCustomer.v1;

internal sealed record UpdateCustomer(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime? BirthDate = null,
    string? DetailAddress = null,
    string? Nationality = null
) : ICommand
{
    /// <summary>
    /// Update the customer with inline validation.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="birthDate"></param>
    /// <param name="detailAddress"></param>
    /// <param name="nationality"></param>
    /// <returns></returns>
    public static UpdateCustomer Of(
        long id,
        string? firstName,
        string? lastName,
        string? email,
        string? phoneNumber,
        DateTime? birthDate = null,
        string? detailAddress = null,
        string? nationality = null
    )
    {
        return new UpdateCustomerValidator().HandleValidation(
            new UpdateCustomer(id, firstName!, lastName!, email!, phoneNumber!, birthDate, detailAddress, nationality)
        );
    }
}

internal class UpdateCustomerValidator : AbstractValidator<UpdateCustomer>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().WithMessage("Email address is invalid.");
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(p => p.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone Number is required.")
            .MinimumLength(7)
            .WithMessage("PhoneNumber must not be less than 7 characters.")
            .MaximumLength(15)
            .WithMessage("PhoneNumber must not exceed 15 characters.");
    }
}

internal class UpdateCustomerHandler(CustomersDbContext customersDbContext, ILogger<UpdateCustomerHandler> logger)
    : ICommandHandler<UpdateCustomer>
{
    public async Task Handle(UpdateCustomer command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating customer");

        command.NotBeNull();

        var customer = await customersDbContext.Customers.FindAsync(
            [CustomerId.Of(command.Id)],
            cancellationToken: cancellationToken
        );

        if (customer is null)
        {
            throw new CustomerNotFoundException(command.Id);
        }

        customer.Update(
            Email.Of(command.Email),
            PhoneNumber.Of(command.PhoneNumber),
            CustomerName.Of(command.FirstName, command.LastName),
            null,
            command.BirthDate == null ? null : BirthDate.Of((DateTime)command.BirthDate),
            command.Nationality == null ? null : Nationality.Of(command.Nationality)
        );

        await customersDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Customer with Id: '{@CustomerId}' updated", customer.Id);

        // TODO: Update Identity user with new customer changes
    }
}
