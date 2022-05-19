using System.Data;
using System.Runtime.CompilerServices;
using Gurgen.Common;
using MySql.Data.MySqlClient;

namespace Gurgen.Readers;

public class MySqlContentEnumerator : IContentEnumerator
{
    private readonly string _connectionString;
    private readonly Func<DataRow, Content> _mapper;
    private readonly string _sqlText;

    public MySqlContentEnumerator(string connectionString, string sqlText, Func<DataRow, Content> mapper)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async IAsyncEnumerable<Content> Enumerate([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var conn = new MySqlConnection(_connectionString);
        try
        {
            await conn.OpenAsync(cancellationToken);
        }
        finally
        {
            if (conn.State != ConnectionState.Closed)
                await conn.CloseAsync(cancellationToken);
        }

        using var adapter = new MySqlDataAdapter("select content from tblFiles", conn);
        var dataSet = new DataSet();
        await adapter.FillAsync(dataSet, cancellationToken);

        foreach (DataRow row in dataSet.Tables[0].Rows)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            var text = row[0] as string;
            yield return new Content(text);
        }

        if (conn.State != ConnectionState.Closed)
            await conn.CloseAsync(cancellationToken);
    }
}