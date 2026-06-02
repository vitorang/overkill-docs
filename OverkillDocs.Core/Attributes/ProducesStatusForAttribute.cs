namespace OverkillDocs.Core.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ProducesStatusForAttribute(params Type[] exceptionTypes) : Attribute
{
    public Type[] ExceptionTypes { get; } = exceptionTypes ?? [];
}
