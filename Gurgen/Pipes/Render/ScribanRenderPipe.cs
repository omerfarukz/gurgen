using Scriban;
using Scriban.Runtime;

namespace Gurgen.Pipes.Render;

public class ScribanRenderPipe : RenderPipeBase
{
    /// <summary>
    ///     https://github.com/scriban/scriban/blob/master/doc/runtime.md
    /// </summary>
    /// <param name="renderContext"></param>
    /// <param name="cancellationToken"></param>
    public override void Render(RenderContext renderContext, CancellationToken cancellationToken)
    {
        // PushGlobal?
        var template = Template.Parse(renderContext.Content.Text);
        var scriptObject = new ScriptObject();
        foreach (var pair in renderContext.Environment.Variables) scriptObject.Add(pair.Key, pair.Value);

        var templateContext = new TemplateContext(scriptObject);
        renderContext.Content.Text = template.Render(templateContext);
    }
}