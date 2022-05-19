using Gurgen.Common;
using Environment = Gurgen.Common.Environment;

namespace Gurgen.Pipes.Render;

public record RenderContext
{
    public readonly Content Content;
    public readonly Environment Environment;

    public RenderContext(Environment environment, string text)
    {
        Environment = environment;
        Content = new Content(text);
    }
}