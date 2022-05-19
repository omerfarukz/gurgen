using CommonMark;

namespace Gurgen.Pipes.Render;

public class MarkdownRenderPipe : RenderPipeBase
{
    public override void Render(RenderContext renderContext, CancellationToken cancellationToken)
    {
        if (renderContext == null)
            throw new ArgumentNullException(nameof(renderContext));
        
        renderContext.Content.Text =
            CommonMarkConverter.Convert(renderContext.Content.Text, CommonMarkSettings.Default);
    }
}