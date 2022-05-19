namespace Gurgen.Pipes.Render;

public abstract class RenderPipeBase : IRenderPipe
{
    public IRenderPipe Next { get; set; }
    public abstract void Render(RenderContext renderContext, CancellationToken cancellationToken);
}