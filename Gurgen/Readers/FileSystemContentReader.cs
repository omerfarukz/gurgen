using Gurgen.Common;

namespace Gurgen.Readers;

public class FileSystemContentReader : IContentReader
{
    private readonly string _filePath;

    public FileSystemContentReader(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public async Task<Content> ReadContent(CancellationToken cancellationToken)
    {
        var text = await File.ReadAllTextAsync(_filePath, cancellationToken);
        return new Content(text);
    }
}