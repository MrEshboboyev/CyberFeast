using BuildingBlocks.Abstractions.Web.Problem;
using Humanizer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Problem;

/// <summary>
/// Handles the creation and writing of problem details for exceptions.
/// </summary>
public class ProblemDetailsService : IProblemDetailsService
{
    private readonly IProblemDetailsWriter[] _writers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProblemDetailsService"/> class.
    /// </summary>
    /// <param name="writers">The problem details writers.</param>
    /// <param name="problemDetailMappers">The problem detail mappers.</param>
    public ProblemDetailsService(
        IEnumerable<IProblemDetailsWriter> writers,
        IEnumerable<IProblemDetailMapper>? problemDetailMappers = null)
    {
        _writers = writers.ToArray();
        ProblemDetailMappers = problemDetailMappers?.ToArray() ?? [];
    }

    /// <summary>
    /// Gets the problem detail mappers.
    /// </summary>
    public IProblemDetailMapper[] ProblemDetailMappers { get; }

    /// <summary>
    /// Writes the problem details to the HTTP response asynchronously.
    /// </summary>
    /// <param name="context">The problem details context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async ValueTask WriteAsync(ProblemDetailsContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(context.ProblemDetails);
        ArgumentNullException.ThrowIfNull(context.HttpContext);

        var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (exceptionFeature is not null)
        {
            CreateProblemDetailFromException(context, exceptionFeature);
        }

        if (context.HttpContext.Response.HasStarted ||
            context.HttpContext.Response.StatusCode < 400 ||
            _writers.Length == 0)
            return;

        if (_writers.Length == 1)
        {
            var writer = _writers[0];
            if (writer.CanWrite(context))
            {
                await writer.WriteAsync(context);
            }

            return;
        }

        var problemDetailsWriter = _writers.FirstOrDefault(writer => writer.CanWrite(context));

        if (problemDetailsWriter != null)
        {
            await problemDetailsWriter.WriteAsync(context);
        }
    }

    #region Private Methods

    private void CreateProblemDetailFromException(
        ProblemDetailsContext context,
        IExceptionHandlerFeature exceptionFeature)
    {
        if (ProblemDetailMappers.Length != 0)
        {
            foreach (var problemDetailMapper in ProblemDetailMappers)
            {
                MapProblemDetail(context, exceptionFeature, problemDetailMapper);
            }
        }
        else
        {
            var defaultMapper = new DefaultProblemDetailMapper();
            MapProblemDetail(context, exceptionFeature, defaultMapper);
        }
    }

    private static void MapProblemDetail(
        ProblemDetailsContext context,
        IExceptionHandlerFeature exceptionFeature,
        IProblemDetailMapper problemDetailMapper)
    {
        var mappedStatusCode = problemDetailMapper.GetMappedStatusCodes(exceptionFeature.Error);
        if (mappedStatusCode <= 0) return;

        PopulateNewProblemDetail(
            context.ProblemDetails,
            context.HttpContext,
            mappedStatusCode,
            exceptionFeature.Error
        );
        context.HttpContext.Response.StatusCode = mappedStatusCode;
    }

    private static void PopulateNewProblemDetail(
        ProblemDetails existingProblemDetails,
        HttpContext httpContext,
        int statusCode,
        Exception exception)
    {
        existingProblemDetails.Title = exception.GetType().Name.Humanize(LetterCasing.Title);
        existingProblemDetails.Detail = exception.Message;
        existingProblemDetails.Status = statusCode;
        existingProblemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
    }

    #endregion
}