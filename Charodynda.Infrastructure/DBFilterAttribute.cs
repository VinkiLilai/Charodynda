using System.Data;
using System.Text;

namespace Charodynda.Infrastructure;

public class DBFilterAttribute : Attribute
{
    public string Name { get; }
    public FilterType Type { get; }
    public string Sql { get; private set; }
    public Func<object, bool> Predicate { get; }

    public DBFilterAttribute(string name, FilterType type)
    {
        if (type != FilterType.Strict && type != FilterType.StrictMany && type != FilterType.Pattern)
            throw new ArgumentException();
        Name = name;
        Type = type;
    }
    
    public DBFilterAttribute(string name, FilterType type, string sql)
    {
        if (type != FilterType.Sql || sql != null || sql != "")
            throw new ArgumentException();
        Name = name;
        Type = type;
        Sql = sql;
    }
    
    public DBFilterAttribute(string name, FilterType type, Func<object, bool> predicate)
    {
        if (type != FilterType.Custom)
            throw new ArgumentException();
        Name = name;
        Type = type;
        Predicate = predicate;
    }

    public string Evaluate(params object[] target)
    {
        if (target is null || target.Length == 0)
            throw new ArgumentException("Target params array must contain something that resulting field will be compared to.");

        switch (Type)
        {
            case FilterType.Strict:
                return $"{Name}={target[0]}";
            
            case FilterType.Pattern:
                return $"{Name} LIKE \'%{target[0]}%\'";
            
            case FilterType.StrictMany:
                return string.Join(" OR ", target.Select(x => $"{Name}={x}"));

            case FilterType.PatternMany:
                return string.Join(" OR ", target.Select(x => $"{Name} LIKE \'%{x}%\'"));

            case FilterType.Sql:
                return Sql.Replace("value", target[0].ToString());
            
            default:
                throw new InvalidOperationException();
        }
    }
    
    
}

public enum FilterType
{
    Strict,
    StrictMany,
    Pattern,
    PatternMany,
    Sql, 
    Custom
}

// WHERE Id = 1
// WHERE Name LIKE "%снаряд%"
// WHERE Class | 5 != 0

// Id, Json

// [DBFilter(FilterType.)]
// public string School { get; }