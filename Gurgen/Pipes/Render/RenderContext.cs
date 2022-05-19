using Gurgen.Common;
using Environment = Gurgen.Common.Environment;

namespace Gurgen.Pipes.Render;

public record RenderContext
{
    public readonly Content Content;

    public RenderContext(Environment environment, string text)
    {
        Environment = environment;
        Content = new Content(text);
    }

    public Environment Environment { get; }
}