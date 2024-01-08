using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Charodynda.Infrastructure;

namespace Charodynda.Domain;

public record struct Spell
{
    public readonly int id;
    [JsonProperty("Name")]
    [DBFilter("Name", FilterType.Pattern)]
    public string Name { get; }
    
    [JsonProperty("Level")]
    [DBFilter("Level", FilterType.StrictMany)]
    public int Level { get; }
    
    [JsonProperty("School")]
    [DBFilter("School", FilterType.StrictMany)]
    public string School { get; }
    
    [JsonProperty("Source")]
    [DBFilter("Source", FilterType.PatternMany)]
    public string Source { get; }
    
    [JsonProperty("CastingTime")]
    public string CastingTime { get; }
    [JsonProperty("Range")]
    public string Range { get; }
    [JsonProperty("Duration")]
    public string Duration { get; }

    [JsonProperty("Components")]
    [DBFilter("Components", FilterType.PatternMany)]
    public string Components { get; }

    [JsonProperty("Materials")]
    public string Materials { get; }
    [JsonProperty("Concentration")]
    [DBFilter("Concentration", FilterType.Strict)]
    public bool Concentration { get; }
    [JsonProperty("Ritual")]
    [DBFilter("Ritual", FilterType.Strict)]
    public bool Ritual { get; }

    [JsonProperty("Class")]
    [DBFilter("Class", FilterType.Sql, "Class | value != 0")]
    public Class Class { get; }

    [JsonProperty("Archetypes")]
    public IReadOnlyCollection<string> Archetypes { get; }

    [JsonProperty("Description")]
    public string Description { get; }
    
    [JsonProperty("DamageType")]
    private DamageType damageType; 
    // Я не совсем уверен, нужен ли нам тип урона отдельным полем, но решил на всякий случай добавить
    // (теоретически - нет, так как эта информация будет получаться из лямбды в объекте Damage).
    
    [JsonProperty("Damage")]
    private Func<int, int, Damage> damage { get; }
    // Урон заклинания зависит от уровня персонажа (в случае заговоров)
    // или от уровня ячейки (в случае заклинаний). Поэтому урон заклинания,
    // по крайней мере в моём понимании, является функцией от этих двух параметров.
    // Лямбда же нужна для того, чтобы сохранять её в базу данных в виде текста,
    // а затем оттуда парсить.

    public Damage Damage(int characterLevel, int spellSlotLevel) 
        => damage(characterLevel, spellSlotLevel);
    // Соответственно, лямбда - это внутренняя информация. Пользователям можно дать красивый API к ней.

    /*public Spell(string name, int level, string school, string source,
        string castingTime, string range, string duration, 
        string components, string materials, bool concentration, bool ritual,
        Class classDesc, IReadOnlyCollection<string> archetypes, string description)
    {
        Name = name;
        Level = level;
        School = school;
        Source = source;
        CastingTime = castingTime;
        Range = range;
        Duration = duration;
        Components = components;
        Materials = materials;
        Concentration = concentration;
        Ritual = ritual;
        Class = classDesc;
        Archetypes = archetypes;
        Description = description;

        damageType = DamageType.None;
        damage = null;
    }*/
    
    public override string ToString()
    {
        var type = GetType();
        var properties = type.GetProperties();

        var result = "";
        foreach (var property in properties)
        {
            if (result != "")
                result += "; ";
            result += property.Name + ": " + property.GetValue(this);
        }
        return result;
    }
}