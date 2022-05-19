using Gurgen.Common;

namespace Gurgen.Pipes.Render;

public record RenderContext
{
    public readonly Content Content;
    public readonly Gurgen.Common.Environment Environment;

    public RenderContext(Gurgen.Common.Environment environment, string text)
    {
        Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        Content = new Content(text);
    }
}