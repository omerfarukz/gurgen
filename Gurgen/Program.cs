using System.Diagnostics;
using Gurgen.Common;
using Gurgen.ContentProviders;
using Gurgen.Pipes;
using Gurgen.Pipes.Render;
using Environment = Gurgen.Common.Environment;

var cancellationToken = CancellationToken.None;

// Setup
var variables = new Dictionary<string, object>
{
    {"Title", "Just another generator"},
    {"Meta-X", "content=foo&kind=bar"}
};
var environment = new Environment(variables);

// Instantiate content providers
var mysqlContentProvider = new MySqlContentProvider(
    "server=localhost;uid=root;pwd=root;database=filedb;Port=3308",
    "select content from tblFiles", 
    row => new Content(row[0] as string)
);
var directoryContentProvider = new DirectoryContentProvider(
    new DirectoryInfo("Assets"), "*.md"
);

// Build pipeline
var renderPipeline = ActionPipe.Empty
    .Then<ScribanRenderPipe>()
    .Then<MarkdownRenderPipe>();

// Process pipeline
await Process(directoryContentProvider);
//await Process(mysqlContentEnumerator);

async Task Process(IContentProvider contentEnumerator)
{
    var pipeline = new Pipeline(
        environment,
        contentEnumerator,
        renderPipeline
    );

    await pipeline.Process(new PipelineOptions(4), cancellationToken);
}