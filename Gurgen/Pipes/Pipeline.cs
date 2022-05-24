using System.Runtime.CompilerServices;
using Gurgen.Common;
using Gurgen.Pipes.Render;
using Environment = Gurgen.Common.Environment;

namespace Gurgen.Pipes;

public record PipelineOptions(int MaxDegreeOfParallelism = 4);

public class Pipeline
{
    private readonly IContentProvider _contentProvider;
    private readonly Environment _environment;
    private readonly IRenderPipe _renderPipe;

    public Pipeline(Environment environment, IContentProvider contentProvider, IRenderPipe renderPipe)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
        _renderPipe = renderPipe ?? throw new ArgumentNullException(nameof(renderPipe));
    }

    public async Task Process(
        PipelineOptions pipelineOptions,
        CancellationToken cancellationToken)
    {
        if (pipelineOptions == null)
            throw new ArgumentNullException(nameof(pipelineOptions));

        var parallelOptions = new ParallelOptions()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = pipelineOptions.MaxDegreeOfParallelism
        };

        await Parallel.ForEachAsync(
            _contentProvider.Enumerate(cancellationToken),
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

    public async IAsyncEnumerable<Content> Process([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var content in _contentProvider.Enumerate(cancellationToken))
        {
            var context = new RenderContext(_environment, content.Text);

            var currentPipe = _renderPipe;
            while (currentPipe != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;
                currentPipe.Render(context, cancellationToken);
                currentPipe = currentPipe.Next;
            }

            yield return context.Content;
        }
    }
}