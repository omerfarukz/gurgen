namespace Gurgen.Common;

public interface IContentEnumerator
{
    IAsyncEnumerable<Content> Enumerate(CancellationToken cancellationToken);
}