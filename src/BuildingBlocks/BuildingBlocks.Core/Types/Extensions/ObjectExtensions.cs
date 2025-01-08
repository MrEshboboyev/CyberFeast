using System.Reflection;
using System.Web;
using BuildingBlocks.Core.Reflection.Extensions;

namespace BuildingBlocks.Core.Types.Extensions;

/// <summary>
/// Provides extension methods for <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Converts the object to a query string.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>The query string representation of the object.</returns>
    public static string GetQueryString(this object obj)
    {
        var properties =
            from p in obj.GetType().GetProperties()
            where p.GetValue(obj, null) != null
            select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null)?.ToString());

        return string.Join("&", properties.ToArray());
    }

    /// <summary>
    /// Invokes a generic method on the object.
    /// </summary>
    /// <param name="instanceObject">The object to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="genericTypes">The generic types to use in the method.</param>
    /// <param name="returnType">The return type of the method (optional).</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>The result of the method invocation.</returns>
    public static dynamic? InvokeGenericMethod(
        this object instanceObject,
        string methodName,
        Type[] genericTypes,
        Type? returnType = null,
        params object[] parameters)
    {
        var method = instanceObject
            .GetType()
            .GetGenericMethod(
                methodName,
                genericTypes,
                parameters.Select(y => y.GetType()).ToArray(),
                returnType);

        if (method == null)
        {
            return null;
        }

        var genericMethod = method.MakeGenericMethod(genericTypes);
        return genericMethod.Invoke(instanceObject, parameters);
    }

    /// <summary>
    /// Invokes an asynchronous generic method on the object.
    /// </summary>
    /// <param name="instanceObject">The object to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="genericTypes">The generic types to use in the method.</param>
    /// <param name="returnType">The return type of the method (optional).</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>A task representing the result of the method invocation.</returns>
    public static Task<dynamic>? InvokeGenericMethodAsync(
        this object instanceObject,
        string methodName,
        Type[] genericTypes,
        Type? returnType = null,
        params object[] parameters)
    {
        var awaitable = InvokeGenericMethod(
            instanceObject,
            methodName,
            genericTypes,
            returnType,
            parameters);

        return awaitable;
    }

    /// <summary>
    /// Invokes a method on the object.
    /// </summary>
    /// <param name="instanceObject">The object to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>The result of the method invocation.</returns>
    public static dynamic InvokeMethod(
        this object instanceObject,
        string methodName,
        params object[] parameters)
    {
        var method = instanceObject
            .GetType()
            .GetMethods(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.Name == methodName)
            .FirstOrDefault(x =>
                x.GetParameters()
                    .Select(p => p.ParameterType)
                    .All(parameters.Select(p => p.GetType()).Contains)
            );

        return method?.Invoke(instanceObject, parameters);
    }

    /// <summary>
    /// Invokes an asynchronous method on the object.
    /// </summary>
    /// <param name="instanceObject">The object to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>A task representing the result of the method invocation.</returns>
    public static Task<dynamic> InvokeMethodAsync(
        this object instanceObject,
        string methodName,
        params object[] parameters)
    {
        var awaitable = InvokeMethod(
            instanceObject,
            methodName,
            parameters);

        return awaitable;
    }

    /// <summary>
    /// Invokes a method on the object without returning the result.
    /// </summary>
    /// <param name="instanceObject">The object to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task InvokeMethodWithoutResultAsync(
        this object instanceObject,
        string methodName,
        params object[] parameters)
    {
        var awaitable = InvokeMethod(
            instanceObject,
            methodName,
            parameters);

        await awaitable;
    }

    /// <summary>
    /// Checks if the type is a value type, primitive type, or string.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns><c>true</c> if the object is a value type, primitive type, or string; otherwise, <c>false</c>.</returns>
    public static bool IsPrimitiveType(this object? obj)
    {
        return obj == null || obj.GetType().IsPrimitiveType();
    }
}