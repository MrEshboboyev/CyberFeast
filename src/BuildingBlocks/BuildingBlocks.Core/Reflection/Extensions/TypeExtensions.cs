using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Core.Reflection.Extensions;

public static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, string> _typeCacheKeys = new();
    private static readonly ConcurrentDictionary<Type, string> _prettyPrintCache = new();

    private const BindingFlags PublicInstanceMembersFlag = BindingFlags.Public |
                                                           BindingFlags.Instance;

    private const BindingFlags AllInstanceMembersFlag =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private const BindingFlags AllStaticAndInstanceMembersFlag =
        PublicInstanceMembersFlag | BindingFlags.NonPublic | BindingFlags.Static;

    /// <summary>
    /// Invoke a static generic method.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <param name="genericTypes"></param>
    /// <param name="returnType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static dynamic? InvokeGenericMethod(
        this Type type,
        string methodName,
        Type[] genericTypes,
        Type? returnType = null,
        params object[] parameters)
    {
        var method = GetGenericMethod(
            type,
            methodName,
            genericTypes,
            parameters.Select(y => y.GetType()).ToArray(),
            returnType
        );

        if (method == null)
        {
            return null;
        }

        var genericMethod = method.MakeGenericMethod(genericTypes);
        return genericMethod.Invoke(null, parameters);
    }

    public static MethodInfo? GetGenericMethod(
        this Type t,
        string name,
        Type[] genericArgTypes,
        Type[] argTypes,
        Type? returnType = null)
    {
        var res = (
            from m in t.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            where
                m.Name == name && 
                m.GetGenericArguments().Length == genericArgTypes.Length && 
                m
                    .GetParameters()
                    .Select(pi => pi.ParameterType)
                    .All(d => argTypes.Any(a => a.IsAssignableTo(d))) &&
                (m.ReturnType == returnType || returnType == null)
            select m
        ).FirstOrDefault();

        return res;
    }
    
    public static Task<dynamic>? InvokeGenericMethodAsync(
        this Type type,
        string methodName,
        Type[] genericTypes,
        Type? returnType = null,
        params object[] parameters)
    {
        var awaitable = InvokeGenericMethod(
            type, 
            methodName,
            genericTypes,
            returnType,
            parameters);

        return awaitable;
    }

    public static dynamic InvokeMethod(this Type type, string methodName, params object[] parameters)
    {
        var method = type
            .GetMethods(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.Name == methodName)
            .FirstOrDefault(x => x
                .GetParameters()
                .Select(p => p.ParameterType)
                .All(parameters.Select(p => p.GetType()).Contains)
            );

        return method is null ? null! : method.Invoke(null, parameters);
    }

    public static Task<dynamic> InvokeMethodAsync(
        this Type type,
        string methodName,
        params object[] parameters)
    {
        var awaitable = InvokeMethod(type, methodName, parameters);

        return awaitable;
    }

    public static string GetCacheKey(this Type type)
    {
        return _typeCacheKeys.GetOrAdd(type, t => $"{t.PrettyPrint()}");
    }

    public static MethodInfo[] GetExtensionMethods(this Type t)
    {
        var assTypes = new List<Type>();

        foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
        {
            assTypes.AddRange(item.GetTypes());
        }

        var query =
            from type in assTypes
            where type.IsSealed &&
                  !type.IsGenericType &&
                  !type.IsNested
            from method in type.GetMethods(
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where method.IsDefined(typeof(ExtensionAttribute), false)
            where method.GetParameters()[0].ParameterType == t
            select method;
        return query.ToArray<MethodInfo>();
    }

    public static MethodInfo GetExtensionMethod(
        this Type t,
        string methodeName)
    {
        var mi = from methode in t.GetExtensionMethods()
            where methode.Name == methodeName select methode;
        return (!mi.Any() 
            ? null 
            : mi.First<MethodInfo>())!;
    }

    public static Type GetPayloadType(
        this ScheduleSerializedObject messageSerializedObject)
    {
        if (messageSerializedObject?.AssemblyName == null)
            return null!;

        var assembly = Assembly.Load(messageSerializedObject.AssemblyName);

        var type = assembly
            .GetTypes()
            .Where(t => t.FullName == messageSerializedObject.FullTypeName)
            .ToList()
            .FirstOrDefault();
        return type!;
    }

    private static string PrettyPrintRecursive(Type type, int depth)
    {
        if (depth > 3)
            return type.Name;

        var nameParts = type.Name.Split('`');
        if (nameParts.Length == 1)
            return nameParts[0];

        var genericArguments = type.GetTypeInfo().GetGenericArguments();
        return !type.IsConstructedGenericType
            ? $"{nameParts[0]}<{new string(',', genericArguments.Length - 1)}>"
            : $"{nameParts[0]}<{string.Join(",", genericArguments
                .Select(t => PrettyPrintRecursive(t, depth + 1)))}>";
    }

    public static IEnumerable<Type> GetAllInterfacesImplementingOpenGenericInterface(
        this Type type,
        Type openGenericType)
    {
        var interfaces = type.GetInterfaces();
        return interfaces.Where(x => x.IsGenericType &&
                                     x.GetGenericTypeDefinition() == openGenericType);
    }

    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        params Assembly[] assemblies)
    {
        var inputAssemblies = assemblies.Length != 0 
            ? assemblies 
            : AppDomain.CurrentDomain.GetAssemblies();
        return inputAssemblies.SelectMany(assembly =>
            GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly)
        );
    }

    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        Assembly assembly)
    {
        try
        {
            return GetAllTypesImplementingOpenGenericInterface(
                openGenericType,
                assembly.GetTypes());
        }
        catch (ReflectionTypeLoadException)
        {
            // It's expected to not being able to load all assemblies
            return new List<Type>();
        }
    }

    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        IEnumerable<Type> types)
    {
        return from type in types
            from interfaceType in type.GetInterfaces()
            where
                interfaceType.IsGenericType &&
                openGenericType.IsAssignableFrom(interfaceType.GetGenericTypeDefinition()) &&
                type.IsClass &&
                !type.IsAbstract
            select type;
    }

    public static IEnumerable<Type> GetAllTypesImplementingInterface(
        this Type interfaceType,
        params Assembly[] assemblies)
    {
        var inputAssemblies = assemblies.Length != 0 
            ? assemblies 
            : AppDomain.CurrentDomain.GetAssemblies();
        return inputAssemblies.SelectMany(assembly => 
            GetAllTypesImplementingInterface(interfaceType, assembly));
    }

    private static IEnumerable<Type> GetAllTypesImplementingInterface(
        this Type interfaceType,
        Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(type =>
                interfaceType.IsAssignableFrom(type) &&
                type is
                {
                    IsInterface: false,
                    IsAbstract: false,
                    IsClass: true
                }
            );
    }

    public static PropertyInfo[] FindPropertiesWithAttribute(
        this Type type,
        Type attribute)
    {
        var properties = type.GetProperties(
            BindingFlags.Public | BindingFlags.Instance);
        return properties.Where(x => x
            .GetCustomAttributes(attribute, true).Length != 0).ToArray();
    }

    public static Type[] GetTypeInheritanceChainTo(this Type type, Type toBaseType)
    {
        var retVal = new List<Type>
        {
            type
        };

        var baseType = type.BaseType;
        while (baseType != toBaseType && baseType != typeof(object))
        {
            retVal.Add(baseType!);
            baseType = baseType?.BaseType;
        }

        return retVal.ToArray();
    }

    public static bool IsDerivativeOf(this Type type, Type typeToCompare)
    {
        ArgumentNullException.ThrowIfNull(type);

        var retVal = type.BaseType != null;
        if (retVal)
        {
            retVal = type.BaseType == typeToCompare;
        }

        if (!retVal && type.BaseType != null)
        {
            retVal = type.BaseType.IsDerivativeOf(typeToCompare);
        }

        return retVal;
    }

    public static T CreateInstanceFromType<T>(
        this Type type,
        params object[] parameters)
    {
        var instance = (T)Activator.CreateInstance(type, parameters)!;

        return instance;
    }

    public static bool IsDictionary(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var retVal = typeof(IDictionary).IsAssignableFrom(type);
        if (!retVal)
        {
            retVal = type.IsGenericType && 
                     typeof(IDictionary<,>)
                         .IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        return retVal;
    }

    public static bool IsAssignableFromGenericList(this Type type)
    {
        foreach (var intType in type.GetInterfaces())
        {
            if (intType.IsGenericType &&
                intType.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsNonAbstractClass(this Type type, bool publicOnly)
    {
        var typeInfo = type.GetTypeInfo();

        if (typeInfo.IsSpecialName)
        {
            return false;
        }

        if (!typeInfo.IsClass || typeInfo.IsAbstract) return false;
        
        if (typeInfo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
        {
            return false;
        }

        if (publicOnly)
        {
            return typeInfo.IsPublic || typeInfo.IsNestedPublic;
        }

        return true;

    }

    public static IEnumerable<Type> GetBaseTypes(this Type type)
    {
        var typeInfo = type.GetTypeInfo();

        foreach (var implementedInterface in typeInfo.ImplementedInterfaces)
        {
            yield return implementedInterface;
        }

        var baseType = typeInfo.BaseType;

        while (baseType != null)
        {
            var baseTypeInfo = baseType.GetTypeInfo();

            yield return baseType;

            baseType = baseTypeInfo.BaseType;
        }
    }

    public static bool IsInNamespace(this Type type, string @namespace)
    {
        var typeNamespace = type.Namespace ?? string.Empty;

        if (@namespace.Length > typeNamespace.Length)
        {
            return false;
        }

        var typeSubNamespace = typeNamespace[..@namespace.Length];

        if (!typeSubNamespace.Equals(@namespace, StringComparison.Ordinal)) return false;
        
        if (typeNamespace.Length == @namespace.Length)
        {
            // exactly the same
            return true;
        }

        // is a subnamespace?
        return typeNamespace[@namespace.Length] == '.';
    }

    public static bool IsInExactNamespace(
        this Type type,
        string @namespace)
    {
        return string.Equals(type.Namespace, @namespace, StringComparison.Ordinal);
    }

    public static bool HasAttribute(this Type type, Type attributeType)
    {
        return type.GetTypeInfo().IsDefined(attributeType, inherit: true);
    }

    public static bool HasAttribute<T>(this Type type, Func<T, bool> predicate)
        where T : Attribute
    {
        return type.GetTypeInfo().GetCustomAttributes<T>(inherit: true).Any(predicate);
    }

    public static bool IsAssignableTo(this Type type, Type otherType)
    {
        var typeInfo = type.GetTypeInfo();
        var otherTypeInfo = otherType.GetTypeInfo();

        return otherTypeInfo.IsGenericTypeDefinition 
            ? typeInfo.IsAssignableToGenericTypeDefinition(otherTypeInfo)
            : otherTypeInfo.IsAssignableFrom(typeInfo);
    }

    private static bool IsAssignableToGenericTypeDefinition(
        this TypeInfo typeInfo,
        TypeInfo genericTypeInfo)
    {
        var interfaceTypes = typeInfo
            .ImplementedInterfaces
            .Select(t => t.GetTypeInfo());

        foreach (var interfaceType in interfaceTypes)
        {
            if (!interfaceType.IsGenericType) continue;
            
            var typeDefinitionTypeInfo = interfaceType
                .GetGenericTypeDefinition().GetTypeInfo();

            if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
            {
                return true;
            }
        }

        if (typeInfo.IsGenericType)
        {
            var typeDefinitionTypeInfo = typeInfo.GetGenericTypeDefinition().GetTypeInfo();

            if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
            {
                return true;
            }
        }

        var baseTypeInfo = typeInfo.BaseType?.GetTypeInfo();

        return baseTypeInfo is not null && 
               baseTypeInfo.IsAssignableToGenericTypeDefinition(genericTypeInfo);
    }

    private static IEnumerable<Type> GetImplementedInterfacesToMap(TypeInfo typeInfo)
    {
        if (!typeInfo.IsGenericType)
        {
            return typeInfo.ImplementedInterfaces;
        }

        return !typeInfo.IsGenericTypeDefinition 
            ? typeInfo.ImplementedInterfaces 
            : FilterMatchingGenericInterfaces(typeInfo);
    }

    private static IEnumerable<Type> FilterMatchingGenericInterfaces(TypeInfo typeInfo)
    {
        var genericTypeParameters = typeInfo.GenericTypeParameters;

        foreach (var current in typeInfo.ImplementedInterfaces)
        {
            var currentTypeInfo = current.GetTypeInfo();

            if (
                currentTypeInfo is
                {
                    IsGenericType: true,
                    ContainsGenericParameters: true
                }
                && GenericParametersMatch(
                    genericTypeParameters, 
                    currentTypeInfo.GenericTypeArguments)
            )
            {
                yield return currentTypeInfo.GetGenericTypeDefinition();
            }
        }
    }

    private static bool GenericParametersMatch(
        IReadOnlyList<Type> parameters,
        IReadOnlyList<Type> interfaceArguments)
    {
        if (parameters.Count != interfaceArguments.Count)
        {
            return false;
        }

        return !parameters.Where(
            (t, i) => t != interfaceArguments[i]).Any();
    }

    public static bool IsOpenGeneric(this Type type)
    {
        return type.GetTypeInfo().IsGenericTypeDefinition;
    }

    public static bool HasMatchingGenericArity(
        this Type interfaceType,
        TypeInfo typeInfo)
    {
        if (!typeInfo.IsGenericType) return true;
        
        var interfaceTypeInfo = interfaceType.GetTypeInfo();

        if (!interfaceTypeInfo.IsGenericType) return false;
        
        var argumentCount = interfaceType.GenericTypeArguments.Length;
        var parameterCount = typeInfo.GenericTypeParameters.Length;

        return argumentCount == parameterCount;
    }

    private static bool IsPrimitive(this Type type)
    {
        if (type == typeof(string))
            return true;
        return type.IsValueType || type.IsPrimitive;
    }

    public static Type UnwrapNullableType(this Type type) => 
        Nullable.GetUnderlyingType(type) ?? type;

    public static bool IsNullableType(this Type type)
    {
        var typeInfo = type.GetTypeInfo();

        return !typeInfo.IsValueType || 
               (typeInfo.IsGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    public static Type UnwrapEnumType(this Type type)
    {
        var isNullable = type.IsNullableType();
        
        var underlyingNonNullableType = isNullable ? type.UnwrapNullableType() : type;
        
        if (!underlyingNonNullableType.GetTypeInfo().IsEnum)
        {
            return type;
        }

        var underlyingEnumType = Enum.GetUnderlyingType(underlyingNonNullableType);
        return isNullable ? MakeNullable(underlyingEnumType) : underlyingEnumType;
    }

    public static Type MakeNullable(this Type type, bool nullable = true) =>
        type.IsNullableType() == nullable
            ? type
            : nullable
                ? typeof(Nullable<>).MakeGenericType(type)
                : type.UnwrapNullableType();

    public static void AddImplementationsAsTransient(
        this Type[] openMessageInterfaces,
        IServiceCollection services,
        IEnumerable<Assembly> assembliesToScan,
        bool addIfAlreadyExists)
    {
        foreach (var openInterface in openMessageInterfaces)
        {
            var concretions = new List<Type>();
            var interfaces = new List<Type>();

            foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes))
            {
                IEnumerable<Type> interfaceTypes = type
                    .FindInterfacesThatClose(openInterface).ToArray();
                
                if (!interfaceTypes.Any())
                    continue;

                if (type.IsConcrete())
                {
                    concretions.Add(type);
                }

                foreach (var interfaceType in interfaceTypes)
                {
                    if (interfaceType.GetInterfaces().Length != 0)
                    {
                        // Register the IRequestHandler instead of ICommand/Query/EventHandler
                        interfaces.AddRange(interfaceType.GetInterfaces());
                    }
                    else
                    {
                        interfaces.Fill(interfaceType);
                    }
                }
            }

            foreach (var @interface in interfaces.Distinct())
            {
                var matches = concretions
                    .Where(t => t.CanBeCastTo(@interface))
                    .ToList();

                if (addIfAlreadyExists)
                {
                    matches.ForEach(match =>
                        services.TryAddTransient(@interface, match));
                }
                else
                {
                    if (matches.Count > 1)
                    {
                        matches.RemoveAll(m => 
                            !IsMatchingWithInterface(m, @interface));
                    }

                    matches.ForEach(match =>
                        services.TryAddTransient(@interface, match));
                }

                if (!@interface.IsOpenGeneric())
                {
                    AddConcretionsThatCouldBeClosed(@interface, concretions, services);
                }
            }
        }
    }

    private static void Fill<T>(this IList<T> list, T value)
    {
        if (list.Contains(value))
            return;
        list.Add(value);
    }

    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        if (givenType == null || genericType == null)
        {
            return false;
        }

        return givenType == genericType
            || givenType.MapsToGenericTypeDefinition(genericType)
            || givenType.HasInterfaceThatMapsToGenericTypeDefinition(genericType)
            || givenType.BaseType!.IsAssignableToGenericType(genericType);
    }

    private static bool HasInterfaceThatMapsToGenericTypeDefinition(
        this Type givenType, 
        Type genericType)
    {
        return givenType
            .GetInterfaces()
            .Where(it => it.IsGenericType)
            .Any(it => it.GetGenericTypeDefinition() == genericType);
    }

    private static bool MapsToGenericTypeDefinition(
        this Type givenType,
        Type genericType)
    {
        return genericType.IsGenericTypeDefinition
            && givenType.IsGenericType
            && givenType.GetGenericTypeDefinition() == genericType;
    }

    public static bool IsEvent(this Type type) => type.IsAssignableTo(typeof(IEvent));

    private static bool IsRecord(this Type objectType)
    {
        return objectType.GetMethod("<Clone>$") != null
            || ((TypeInfo)objectType)
                .DeclaredProperties.FirstOrDefault(x => 
                x.Name == "EqualityContract")
                ?.GetMethod?.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null;
    }

    private static bool IsMatchingWithInterface(Type handlerType, Type handlerInterface)
    {
        while (true)
        {
            if (handlerType == null || handlerInterface == null)
            {
                return false;
            }

            if (handlerType.IsInterface)
            {
                if (handlerType
                    .GenericTypeArguments
                    .SequenceEqual(handlerInterface.GenericTypeArguments))
                {
                    return true;
                }
            }
            else
            {
                handlerType = handlerType.GetInterface(handlerInterface.Name)!;
                continue;
            }

            return false;
        }
    }

    private static void AddConcretionsThatCouldBeClosed(
        Type @interface,
        List<Type> concretions,
        IServiceCollection services)
    {
        foreach (var type in concretions
                .Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
        {
            services.TryAddTransient(
                @interface,
                type.MakeGenericType(@interface.GenericTypeArguments));
        }
    }

    public static bool CouldCloseTo(
        this Type openConcretion,
        Type closedInterface)
    {
        var openInterface = closedInterface.GetGenericTypeDefinition();
        var arguments = closedInterface.GenericTypeArguments;

        var concreteArguments = openConcretion.GenericTypeArguments;
        return arguments.Length == concreteArguments.Length && 
               openConcretion.CanBeCastTo(openInterface);
    }

    public static bool CanBeCastTo(this Type pluggedType, Type pluginType)
    {
        if (pluggedType == null)
            return false;

        return pluggedType == pluginType
               || pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo());
    }

    public static IEnumerable<Type> FindInterfacesThatClose(
        this Type pluggedType,
        Type templateType)
    {
        if (!pluggedType.IsConcrete())
            yield break;

        if (templateType.GetTypeInfo().IsInterface)
        {
            foreach (
                var interfaceType in pluggedType
                    .GetTypeInfo()
                    .ImplementedInterfaces.Where(type =>
                        type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == templateType)
                    )
            )
            {
                yield return interfaceType;
            }
        }
        else if (
            pluggedType.GetTypeInfo().BaseType!.GetTypeInfo().IsGenericType
            && (pluggedType.GetTypeInfo().BaseType!.GetGenericTypeDefinition() == templateType)
        )
        {
            yield return pluggedType.GetTypeInfo().BaseType!;
        }

        if (pluggedType == typeof(object))
            yield break;
        if (pluggedType.GetTypeInfo().BaseType == typeof(object))
            yield break;

        foreach (var interfaceType in FindInterfacesThatClose(pluggedType.GetTypeInfo().BaseType!, templateType))
        {
            yield return interfaceType;
        }
    }

    public static bool IsConcrete(this Type type)
    {
        return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
    }

    public static Type GetRegistrationType(this Type interfaceType, TypeInfo typeInfo)
    {
        if (!typeInfo.IsGenericTypeDefinition) return interfaceType;
        
        var interfaceTypeInfo = interfaceType.GetTypeInfo();

        return interfaceTypeInfo.IsGenericType 
            ? interfaceType.GetGenericTypeDefinition() 
            : interfaceType;
    }

    public static string GetModuleName(this Type type)
    {
        if (type?.Namespace is null)
            return string.Empty;
        var moduleName = type.Assembly.GetName().Name;
        return type.Namespace.StartsWith(moduleName!, StringComparison.Ordinal)
            ? type.Namespace.Split(".")[2].ToLowerInvariant()
            : string.Empty;
    }

    public static string PrettyPrint(this Type type)
    {
        return _prettyPrintCache.GetOrAdd(
            type,
            t =>
            {
                try
                {
                    return PrettyPrintRecursive(t, 0);
                }
                catch (System.Exception)
                {
                    return t.Name;
                }
            }
        );
    }

    public static bool HasAggregateApplyMethod<TDomainEvent>(this Type type)
    {
        return type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Any(mi =>
                mi.Name == "Apply"
                && mi.GetParameters().Length == 1
                && typeof(TDomainEvent)
                    .GetTypeInfo()
                    .IsAssignableFrom(mi.GetParameters()[0].ParameterType)
            );
    }

    public static bool HasAggregateApplyMethod(this Type type, Type eventType)
    {
        return type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Any(mi =>
                mi.Name == "Apply"
                && mi.GetParameters().Length == 1
                && eventType
                    .GetTypeInfo()
                    .IsAssignableFrom(mi.GetParameters()[0].ParameterType)
            );
    }

    public static IReadOnlyDictionary<Type, Action<TDomainEvent>> GetAggregateApplyMethods<TDomainEvent>(this Type type)
        where TDomainEvent : IDomainEvent
    {
        var aggregateEventType = typeof(TDomainEvent);

        return type.GetTypeInfo()
            .GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(mi =>
            {
                if (
                    !string.Equals(mi.Name, "Apply", StringComparison.Ordinal)
                    && !mi.Name.EndsWith(".Apply", StringComparison.Ordinal)
                )
                {
                    return false;
                }

                var parameters = mi.GetParameters();
                return parameters.Length == 1
                    && aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
            })
            .ToDictionary(
                mi => mi.GetParameters()[0].ParameterType,
                mi => type.CompileMethodInvocation<Action<TDomainEvent>>(mi.Name, mi.GetParameters()[0].ParameterType)
            );
    }

    /// <summary>
    /// Handles correct upcast. If no upcast was needed, then this could be exchanged to an <c>Expression.Call</c>
    /// and an <c>Expression.Lambda</c>.
    /// </summary>
    public static TResult CompileMethodInvocation<TResult>(
        this Type type,
        string methodName,
        params Type[] methodSignature
    )
    {
        var typeInfo = type.GetTypeInfo();
        var methods = typeInfo
            .GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.Name == methodName);

        var methodInfo =
            methodSignature == null || methodSignature.Length == 0
                ? methods.SingleOrDefault()
                : methods.SingleOrDefault(m =>
                    m.GetParameters()
                        .Select(mp => mp.ParameterType)
                        .SequenceEqual(methodSignature)
                );

        if (methodInfo == null)
        {
            throw new ArgumentException($"Type '{type.PrettyPrint()}' doesn't have a method called '{methodName}'");
        }

        return ReflectionUtilities.CompileMethodInvocation<TResult>(methodInfo);
    }

    public static PropertyInfo? FindProperty(this Type type, string propertyName)
    {
        PropertyInfo? res = null;
        foreach (var prop in propertyName.Split('.'))
        {
            res = res == null 
                ? type.GetProperty(prop)
                : res.PropertyType.GetProperty(prop);
        }

        return res;
    }
}
