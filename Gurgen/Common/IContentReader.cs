namespace Gurgen.Common;

public interface IContentReader
{
    Task<Content> ReadContent(CancellationToken cancellationToken);
}