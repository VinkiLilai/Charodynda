using System.Data;
using System.Text;

namespace Charodynda.Infrastructure;

public class DBFilterAttribute : Attribute
{
    public string Name { get; }
    public FilterType Type { get; }
    public string Sql { get; }
    public Func<object, bool> Predicate { get; }

    public DBFilterAttribute(string name, FilterType type)
    {
        if (type == FilterType.Sql || type == FilterType.Custom)
            throw new ArgumentException("Sql and Custom filters require additional parametres and cannot be initiated with this constructor.");
        Name = name;
        Type = type;
    }
    
    public DBFilterAttribute(string name, FilterType type, string sql)
    {
        if (type is not FilterType.Sql)
            throw new ArgumentException("This constructor is only used to initialisse Sql filters.");
        if (sql is null or "")
            throw new ArgumentException("Sql must be initialised to use this filter.");
        Name = name;
        Type = type;
        Sql = sql;
    }
    
    public DBFilterAttribute(string name, FilterType type, Func<object, bool> predicate)
    {
        throw new NotImplementedException("Don't use this one, it's not quite finished.");
        
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
                return $"{Name}={FormatEntry(target[0])}";
            
            case FilterType.Pattern:
                return $"{Name} LIKE \'%{target[0]}%\'";
            
            case FilterType.StrictMany:
                return string.Join(" OR ", target.Select(x => $"{Name}={FormatEntry(x)}"));

            case FilterType.PatternMany:
                return string.Join(" OR ", target.Select(x => $"{Name} LIKE \'%{x}%\'"));

            case FilterType.Sql:
                return Sql.Replace("value", target[0].ToString());
            
            default:
                throw new InvalidOperationException();
        }
    }

    private string FormatEntry(object entry) 
        => entry is string ? $"'{entry}'" : entry.ToString();
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