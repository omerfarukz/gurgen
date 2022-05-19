using Gurgen.Common;
using Gurgen.Pipes.Render;
using Environment = Gurgen.Common.Environment;

namespace Gurgen.Pipes;

public record PipelineOptions(int MaxDegreeOfParallelism = 4);

public class Pipeline
{
    private readonly IContentEnumerator _contentEnumerator;
    private readonly Environment _environment;
    private readonly IRenderPipe _renderPipe;

    public Pipeline(Environment environment, IContentEnumerator contentEnumerator, IRenderPipe renderPipe)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _contentEnumerator = contentEnumerator ?? throw new ArgumentNullException(nameof(contentEnumerator));
        _renderPipe = renderPipe ?? throw new ArgumentNullException(nameof(renderPipe));
    }

    public async Task Process(PipelineOptions pipelineOptions, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = pipelineOptions.MaxDegreeOfParallelism
        };
        
        await Parallel.ForEachAsync(
            _contentEnumerator.Enumerate(cancellationToken), 
            parallelOptions,
            (content, token) =>
            {
                var context = new RenderContext(_environment, content.Text);

                var currentPipe = _renderPipe;
                while (currentPipe != null)
                {
                    currentPipe.Render(context, token);
                    currentPipe = currentPipe.Next;
                }
                return ValueTask.CompletedTask;
            });
    }

    private async Task ProcessSync(CancellationToken cancellationToken)
    {
        await foreach (var content in _contentEnumerator.Enumerate(cancellationToken))
        {
            var context = new RenderContext(_environment, content.Text);

            var currentPipe = _renderPipe;
            while (currentPipe != null)
            {
                currentPipe.Render(context, cancellationToken);
                currentPipe = currentPipe.Next;
            }
        }
    }
}