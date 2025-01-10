using BuildingBlocks.Abstractions.Scheduler;
using BuildingBlocks.Core.Reflection.Extensions;
using MediatR;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Extensions;

/// <summary>
/// Provides extension methods for the IMediator interface to send scheduled objects.
/// </summary>
public static class MediatRExtensions
{
    /// <summary>
    /// Sends a scheduled serialized object as a dynamic request using MediatR.
    /// </summary>
    /// <param name="mediator">The IMediator instance.</param>
    /// <param name="scheduleSerializedObject">The scheduled serialized object.</param>
    public static async Task SendScheduleObject(
        this IMediator mediator,
        ScheduleSerializedObject scheduleSerializedObject
    )
    {
        var type = scheduleSerializedObject.GetPayloadType();
        dynamic? req = JsonConvert.DeserializeObject(scheduleSerializedObject.Data, type);

        if (req is null)
        {
            return;
        }

        await mediator.Send(req);
    }
}