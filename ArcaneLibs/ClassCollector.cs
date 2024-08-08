using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ArcaneLibs;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class ClassCollector<T> where T : class {
    static ClassCollector() {
        if (!typeof(T).IsInterface && !typeof(T).IsAbstract)
            throw new ArgumentException($"ClassCollector<T> must be used with an interface or abstract type. Passed type: {typeof(T).Name}");
    }

    public static IEnumerable<Type> ResolveFromAllAccessibleAssemblies() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(ResolveFromAssembly);

    public static IEnumerable<Type> ResolveFromObjectReference(object obj) => ResolveFromTypeReference(obj.GetType());

    public static IEnumerable<Type> ResolveFromTypeReference(Type t) => Assembly.GetAssembly(t)?.GetReferencedAssemblies().SelectMany(ResolveFromAssemblyName) ?? [];

    public static IEnumerable<Type> ResolveFromAssemblyName(AssemblyName assemblyName) => ResolveFromAssembly(Assembly.Load(assemblyName));

    public static IEnumerable<Type> ResolveFromAssembly(Assembly assembly) => assembly.GetTypes()
        .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsAssignableTo(typeof(T)));
}