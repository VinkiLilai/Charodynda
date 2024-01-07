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
    static DatabaseApi()
    {
        properties = typeof(T).GetProperties().Where(
            property => property.GetCustomAttributes(typeof(DBFilterAttribute)).Any()
        ).ToArray();
        
        fields = typeof(T).GetFields().Where(
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
    }

    public IEnumerable<T> GetAll(string table)
    {
        var selectQuery = $"SELECT Json FROM {table}";
        using var query = new SQLiteCommand(selectQuery, connection);
        using var reader = query.ExecuteReader();
        
        while (reader.Read())
            yield return JsonConvert.DeserializeObject<T>((string)reader["Json"]);
        //serializer.Serialize( Enumerable.Range(0, reader.FieldCount - 1).ToDictionary(i => reader.GetName(i), i => reader.GetValue(i)) );
    }
    
    public T FindById(string table, int id)
    {
        var selectQuery = $"SELECT Json FROM {table} WHERE Id = {id}";
        using var query = new SQLiteCommand(selectQuery, connection);
        using var reader = query.ExecuteReader();
        
        return JsonConvert.DeserializeObject<T>((string)reader["Json"]);
    }

    public IEnumerable<T> FindByFilter(string table, Dictionary<string, object[]> filterValues)
    {
        if (filterValues is null)
            throw new ArgumentException();
        
        if (filterValues.Count == 0)
        {
            foreach (var result in GetAll(table))
                yield return result;
            yield break;
        }
        
        var queryBuilder = new StringBuilder($"SELECT Json FROM {table}");
        var wherePieces = filterValues.Select(kvp =>
            filters.TryGetValue(kvp.Key, out var filterAttr) ? filterAttr.Evaluate(kvp.Value) : "").ToArray();

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

    public void Update(string table, int id, T newObj)
    {
        var propNames = string.Join(',', properties.Select(p => p.Name));
        var propValues = string.Join(',', properties.Select(p => p.GetValue(newObj)));

        var fieldNames = string.Join(',', fields.Select(f => f.Name));
        var fieldValues = string.Join(',', fields.Select(f => f.GetValue(newObj)));
        
        var json = JsonConvert.SerializeObject(newObj);
        
        var insertionQuery = $"REPLACE INTO {table} (Id,{propNames},{fieldNames},Json) VALUES ({id},{propValues},{fieldValues},{json});";
        using var query = new SQLiteCommand(insertionQuery, connection);
        query.ExecuteNonQuery();
    }

    public void RemoveById(string table, int id)
    {
        var deletionQuery = $"DELETE FROM {table} WHERE Id={id};";
        using var query = new SQLiteCommand(deletionQuery, connection);
        query.ExecuteNonQuery();
    }
    
    public void Add(string table, T obj)
    {
        var propNames = string.Join(',', properties.Select(p => p.Name));
        var propValues = string.Join(',', properties.Select(p => p.GetValue(obj)));

        var fieldNames = string.Join(',', fields.Select(f => f.Name));
        var fieldValues = string.Join(',', fields.Select(f => f.GetValue(obj)));
        
        var json = JsonConvert.SerializeObject(obj);
        
        var insertionQuery = $"INSERT INTO {table} ({propNames},{fieldNames},Json) VALUES ({propValues},{fieldValues},{json});";
        using var query = new SQLiteCommand(insertionQuery, connection);
        query.ExecuteNonQuery();
    }

    public void Dispose()
    {
        connection.Close();
        connection.Dispose();
    }
}