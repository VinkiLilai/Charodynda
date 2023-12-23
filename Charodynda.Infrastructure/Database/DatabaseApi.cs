using System.Data.SQLite;
using System.Reflection;

namespace Charodynda.Infrastructure.Database;

public class DatabaseApi<T> : IDatabaseApi<T>, IDisposable
{
    private static PropertyInfo[] properties;
    
    static DatabaseApi()
    {
        properties = typeof(T).GetProperties();
    }

    private string database;
    private SQLiteConnection connection;
    private IObjectSerializer<T> serializer;

    public DatabaseApi(string databaseName, IObjectSerializer<T> serializer)
    {
        database = databaseName;
        connection = new SQLiteConnection($"Data Source={database};Version=3;");
        connection.Open();

        this.serializer = serializer;
    }
    
    public T FindById(string table, int id)
    {
        var selectQuery = $"SELECT * FROM {table} WHERE Id = {id}";
        using var query = new SQLiteCommand(selectQuery, connection);
        using var reader = query.ExecuteReader();
        
        return serializer.Serialize( Enumerable.Range(0, reader.FieldCount - 1)
            .ToDictionary(i => reader.GetName(i), i => reader.GetValue(i)) );
    }

    public IEnumerable<int> FindIdsByFilter(string table, IFilter<T> filter)
    {
        var selectQuery = $"SELECT Id FROM {table} WHERE {filter.ToSQLConstraints()}";
        using var query = new SQLiteCommand(selectQuery, connection);
        using var reader = query.ExecuteReader();
        while (reader.Read())
            yield return Convert.ToInt32(reader["Id"]);
    }

    public void RemoveById(string table, int id)
    {
        var deletionQuery = $"DELETE FROM {table} WHERE Id={id};";
        using var query = new SQLiteCommand(deletionQuery, connection);
        query.ExecuteNonQuery();
    }
    
    public void Add(string table, T obj)
    {
        var values = properties.Select(p => p.GetValue(obj));
        var insertionQuery = $"INSERT INTO {table} ({string.Join(',', properties.Select(p => p.Name))}) VALUES ({string.Join(',', values)});";
        using var query = new SQLiteCommand(insertionQuery, connection);
        query.ExecuteNonQuery();
    }

    public IEnumerable<T> GetAll(string table)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        connection.Close();
        connection.Dispose();
    }
}