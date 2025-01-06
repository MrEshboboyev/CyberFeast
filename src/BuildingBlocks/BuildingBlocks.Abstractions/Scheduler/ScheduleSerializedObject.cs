namespace BuildingBlocks.Abstractions.Scheduler;

/// <summary>
/// Represents a serialized object to be scheduled.
/// </summary>
/// <param name="fullTypeName">The full type name of the object.</param>
/// <param name="data">The serialized data of the object.</param>
/// <param name="additionalDescription">Additional description of the object.</param>
/// <param name="assemblyName">The name of the assembly containing the object.</param>
public class ScheduleSerializedObject(
    string fullTypeName,
    string data,
    string additionalDescription,
    string assemblyName)
{
    public string FullTypeName { get; } = fullTypeName;
    public string Data { get; private set; } = data;
    public string AdditionalDescription { get; } = additionalDescription;
    public string AssemblyName { get; private set; } = assemblyName;

    public override string ToString()
    {
        var commandName = FullTypeName.Split('.').Last();
        return $"{commandName} {AdditionalDescription}";
    }
}
