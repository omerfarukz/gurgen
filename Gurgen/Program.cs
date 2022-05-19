using System.Diagnostics;
using Gurgen.Common;
using Gurgen.Pipes;
using Gurgen.Pipes.Render;
using Gurgen.Readers;
using Environment = Gurgen.Common.Environment;

var cancellationToken = CancellationToken.None;

// Setup
var variables = new Dictionary<string, object>
{
    {"Title", "Just another generator"},
    {"Meta-X", "content=foo&kind=bar"}
};
var environment = new Environment(variables);

// Content enumerators
var mysqlContentEnumerator = new MySqlContentEnumerator(
    "server=localhost;uid=root;pwd=root;database=filedb;Port=3308",
    "select content from tblFiles", row => new Content(row[0] as string)
);
var directoryContentEnumerator = new DirectoryContentEnumerator(
    new DirectoryInfo("Assets"), "*.md"
);

// Pipeline
var renderPipeline = ActionPipe.Empty
    .Then<ScribanRenderPipe>()
    .Then<MarkdownRenderPipe>();

var dateStarted = DateTimeOffset.Now;
await Process(directoryContentEnumerator);
Console.WriteLine((DateTimeOffset.Now - dateStarted).TotalMilliseconds);
//await Process(mysqlContentEnumerator);

async Task Process(IContentEnumerator contentEnumerator)
{
    var pipeline = new Pipeline(
        environment,
        contentEnumerator,
        renderPipeline
    );

    await pipeline.Process(new PipelineOptions(4), cancellationToken);
}