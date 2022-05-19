using Gurgen.Pipes.Render;

namespace Gurgen.Pipes;

public static class PipeExtensions
{
    public static IRenderPipe Then<T>(this IRenderPipe current)
        where T : IRenderPipe, new()
    {
        var mainPipe = current;
        var lastPipe = current;
        while (lastPipe.Next != null)
            lastPipe = lastPipe.Next;

        var nextRenderPipe = Activator.CreateInstance<T>();
        lastPipe.Next = nextRenderPipe;
        return mainPipe;
    }
}