using Gurgen.Pipes.Render;

namespace Gurgen.Pipes;

public class ActionPipe : RenderPipeBase
{
    public static readonly ActionPipe Empty = new(c => { });
    private readonly Action<RenderContext> _action;

    public ActionPipe(Action<RenderContext> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public override void Render(RenderContext renderContext, CancellationToken cancellationToken)
    {
        _action(renderContext);
    }
}