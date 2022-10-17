using System.Runtime.CompilerServices;
using Gurgen.Common;

namespace Gurgen.ContentProviders;

public class DirectoryContentProvider : IContentProvider
{
    private readonly DirectoryInfo _directoryInfo;
    private readonly string _searchPattern;

    public DirectoryContentProvider(DirectoryInfo directoryInfo, string searchPattern)
    {
        _directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
        _searchPattern = searchPattern ?? throw new ArgumentNullException(nameof(searchPattern));
    }

    public async IAsyncEnumerable<Content> Enumerate([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!_directoryInfo.Exists)
            throw new DirectoryNotFoundException();

        foreach (var fileInfo in _directoryInfo.EnumerateFiles(_searchPattern))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var text = await File.ReadAllTextAsync(fileInfo.FullName, cancellationToken);
            yield return new Content(text);
        }
    }
}