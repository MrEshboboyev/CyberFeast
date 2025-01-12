using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web;

/// <summary>
/// An abstract controller that provides common services such as IMediator, ICommandBus, IQueryBus, and IMapper.
/// </summary>
[ApiController]
public abstract class BaseController : Controller
{
    protected const string BaseApiPath = Constants.BaseApiPath;
    private IMapper? _mapper;
    private IMediator? _mediator;
    private ICommandBus? _commandBus;
    private IQueryBus? _queryBus;

    /// <summary>
    /// Gets the IMediator service.
    /// </summary>
    protected IMediator? Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    /// <summary>
    /// Gets the IQueryBus service.
    /// </summary>
    protected IQueryBus? QueryProcessor =>
        _queryBus ??= HttpContext.RequestServices.GetService<IQueryBus>();

    /// <summary>
    /// Gets the ICommandBus service.
    /// </summary>
    protected ICommandBus? CommandBus =>
        _commandBus ??= HttpContext.RequestServices.GetService<ICommandBus>();

    /// <summary>
    /// Gets the IMapper service.
    /// </summary>
    protected IMapper? Mapper =>
        _mapper ??= HttpContext.RequestServices.GetService<IMapper>();
}