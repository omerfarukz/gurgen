namespace Gurgen.Common;

public interface IContentProvider
{
    IAsyncEnumerable<Content> Enumerate(CancellationToken cancellationToken);
}