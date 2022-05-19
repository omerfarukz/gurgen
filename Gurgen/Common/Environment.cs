namespace Gurgen.Common;

public record Environment(Dictionary<string, object> Variables)
{
    public readonly Dictionary<string, object> Variables = Variables;
}