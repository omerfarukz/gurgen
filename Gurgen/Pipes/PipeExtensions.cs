using Gurgen.Pipes.Render;

namespace Gurgen.Pipes;

public static class PipeExtensions
{
    public static IRenderPipe Then<T>(this IRenderPipe current)
        where T : IRenderPipe, new()
    {
        if (current == null)
            throw new ArgumentNullException(nameof(current));
        
        var next = Activator.CreateInstance<T>();
        return SetNextPipe(current, next);
    }

    public static IRenderPipe Then(this IRenderPipe current, Action<RenderContext, CancellationToken> action)
    {
        var pipe = new ActionPipe(action);
        return SetNextPipe(current, pipe);
    }
    
    private static IRenderPipe SetNextPipe(IRenderPipe current, IRenderPipe nextPipeInstance)
    {
        var mainPipe = current;
        var lastPipe = current;
        while (lastPipe.Next != null)
            lastPipe = lastPipe.Next;

        lastPipe.Next = nextPipeInstance;
        return mainPipe;
    }
}