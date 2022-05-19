namespace Gurgen.Pipes.Render;

public interface IRenderPipe
{
    IRenderPipe Next { get; set; }
    void Render(RenderContext renderContext, CancellationToken cancellationToken);
}