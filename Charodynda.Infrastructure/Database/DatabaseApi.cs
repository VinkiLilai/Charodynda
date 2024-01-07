using System.Data.SQLite;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Charodynda.Infrastructure.Database;

public class DatabaseApi<T> : IDatabaseApi<T>, IDisposable
{
    private static PropertyInfo[] properties;
    private static FieldInfo[] fields;
    private static Dictionary<string, DBFilterAttribute> filters;
    private static string tableName;
    private static string idField = null;
    
    static DatabaseApi()
    {
        var type = typeof(T);

        tableName = $"\"{type.Name}\"";
        
        properties = type.GetProperties().Where(
            property => property.GetCustomAttributes(typeof(DBFilterAttribute)).Any()
        ).ToArray();
        
        fields = type.GetFields().Where(
            field => field.GetCustomAttributes(typeof(DBFilterAttribute)).Any()
        ).ToArray();

        filters = properties.ToDictionary(prop => prop.Name,
            prop => (DBFilterAttribute)prop.GetCustomAttributes(typeof(DBFilterAttribute)).First());
        foreach (var (name, attr) in fields.ToDictionary(field => field.Name,
                     field => (DBFilterAttribute)field.GetCustomAttributes(typeof(DBFilterAttribute)).First()))
            filters.Add(name, attr);
    }

    private string database;
    private SQLiteConnection connection;

    public DatabaseApi(string databaseName)
    {
        database = databaseName;
        connection = new SQLiteConnection($"Data Source={database};Version=3;");
        connection.Open();

        if (idField is not null) return;
        
        using var query = new SQLiteCommand($"PRAGMA table_info({tableName})", connection);
        var reader = query.ExecuteReader();

        while (reader.Read())
        {
            var name = reader.GetString(1);
            if (reader.GetInt32(5) != 1) continue;
            idField = name;
            break;
        }
    }

    public IEnumerable<T> GetAll()
    {
        var selectQuery = $"SELECT Json FROM {tableName}";
        using var query = new SQLiteCommand(selectQuery, connection);
        using var reader = query.ExecuteReader();
        
        while (reader.Read())
            yield return JsonConvert.DeserializeObject<T>((string)reader["Json"]);
        //serializer.Serialize( Enumerable.Range(0, reader.FieldCount - 1).ToDictionary(i => reader.GetName(i), i => reader.GetValue(i)) );
    }
    
    public T GetById(int id)
    {
        var selectQuery = $"SELECT Json FROM {tableName} WHERE {idField}={id}";
        using var query = new SQLiteCommand(selectQuery, connection);
        using var reader = query.ExecuteReader();
        
        return JsonConvert.DeserializeObject<T>((string)reader["Json"]);
    }

    public IEnumerable<T> GetByFilter(Dictionary<string, object[]> filterValues)
    {
        if (filterValues is null)
            throw new ArgumentException("Filters cannot be null.");
        
        if (filterValues.Count == 0)
        {
            foreach (var result in GetAll())
                yield return result;
            yield break;
        }
        
        var queryBuilder = new StringBuilder($"SELECT Json FROM {tableName}");
        var wherePieces = filterValues.Select(kvp =>
            filters.TryGetValue(kvp.Key, out var filterAttr) ? filterAttr.Evaluate(kvp.Value) : "").Where(piece => piece.Any()).ToArray();

        if (wherePieces.Length > 0)
        {
            queryBuilder.Append(" WHERE ");
            queryBuilder.Append(string.Join(" AND ", wherePieces));
        }

        using var query = new SQLiteCommand(queryBuilder.ToString(), connection);
        using var reader = query.ExecuteReader();
        while (reader.Read())
            yield return JsonConvert.DeserializeObject<T>((string)reader["Json"]);
    }

    public void Update(int id, T newObj)
    {
        var propNames = string.Join(',', properties.Select(p => p.Name));
        var propValues = string.Join(',', properties.Select(p => FormatEntry(p.GetValue(newObj))));

        var fieldNames = string.Join(',', fields.Select(f => f.Name));
        var fieldValues = string.Join(',', fields.Select(f => FormatEntry(f.GetValue(newObj))));
        
        var json = JsonConvert.SerializeObject(newObj);
        var names = string.Join(',', string.Join(',', idField, propNames , fieldNames, "Json").Split(',', StringSplitOptions.RemoveEmptyEntries));
        var values = string.Join(',', string.Join(',', id.ToString(), propValues, fieldValues, $"\'{json}\'").Split(',', StringSplitOptions.RemoveEmptyEntries));
        
        var insertionQuery = $"REPLACE INTO {tableName} ({names}) VALUES ({values});";
        using var query = new SQLiteCommand(insertionQuery, connection);
        query.ExecuteNonQuery();
    }

    public void RemoveById(int id)
    {
        var deletionQuery = $"DELETE FROM {tableName} WHERE {idField}={id};";
        using var query = new SQLiteCommand(deletionQuery, connection);
        query.ExecuteNonQuery();
    }

    public void Add(T obj)
    {
        var propNames = string.Join(',', properties.Select(p => p.Name));
        var propValues = string.Join(',', properties.Select(p => FormatEntry(p.GetValue(obj))));

        var fieldNames = string.Join(',', fields.Select(f => f.Name));
        var fieldValues = string.Join(',', fields.Select(f => FormatEntry(f.GetValue(obj))));

        var json = JsonConvert.SerializeObject(obj);
        var names = string.Join(',', string.Join(',', propNames , fieldNames, "Json").Split(',', StringSplitOptions.RemoveEmptyEntries));
        var values = string.Join(',', string.Join(',', propValues, fieldValues, $"\'{json}\'").Split(',', StringSplitOptions.RemoveEmptyEntries));

        var insertionQuery = $"INSERT INTO {tableName} ({names}) VALUES ({values});";
        using var query = new SQLiteCommand(insertionQuery, connection);
        query.ExecuteNonQuery();
    }

    public void Dispose()
    {
        connection.Close();
        connection.Dispose();
    }

    private string FormatEntry(object entry)
    {
        if (entry is null)
            throw new ArgumentException("Object properties better not be null :)");
        return entry is string ? $"'{entry}'" : entry.ToString();
    }
}