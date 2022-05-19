using System.Data;
using System.Runtime.CompilerServices;
using Gurgen.Common;
using MySql.Data.MySqlClient;

namespace Gurgen.ContentProviders;

public class MySqlContentProvider : IContentProvider
{
    private readonly string _connectionString;
    private readonly Func<DataRow, Content> _mapper;
    private readonly string _sqlText;

    public MySqlContentProvider(string connectionString, string sqlText, Func<DataRow, Content> mapper)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _sqlText = sqlText ?? throw new ArgumentNullException(nameof(sqlText));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async IAsyncEnumerable<Content> Enumerate([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var connection = new MySqlConnection(_connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
                await connection.CloseAsync(cancellationToken);
        }

        using var adapter = new MySqlDataAdapter(_sqlText, connection);
        var dataSet = new DataSet();
        await adapter.FillAsync(dataSet, cancellationToken);

        foreach (DataRow row in dataSet.Tables[0].Rows)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            var text = row[0] as string;
            yield return new Content(text);
        }

        if (connection.State != ConnectionState.Closed)
            await connection.CloseAsync(cancellationToken);
    }
}