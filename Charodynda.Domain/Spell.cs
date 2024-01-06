using System;
using System.Reflection;

namespace Charodynda.Domain;

public record Spell
{
    public string Name { get; }
    public int Level { get; }
    public string School { get; }
    public string Source { get; }
    
    public string CastingTime { get; }
    public string Range { get; }
    public string Duration { get; }
    
    public IReadOnlyCollection<char> Components;
    public IReadOnlyCollection<string> Materials;
    public bool Concetration { get; }
    public bool Ritual { get; }
    
    public Classes Classes;
    public IReadOnlyCollection<string> Archetypes;

    public string Descripton { get; }
    
    private DamageType damageType; 
    // Я не совсем уверен, нужен ли нам тип урона отдельным полем, но решил на всякий случай добавить
    // (теоретически - нет, так как эта информация будет получаться из лямбды в объекте Damage).
    
    private Func<int, int, Damage> damage { get; }
    // Урон заклинания зависит от уровня персонажа (в случае заговоров)
    // или от уровня ячейки (в случае заклинаний). Поэтому урон заклинания,
    // по крайней мере в моём понимании, является функцией от этих двух параметров.
    // Лямбда же нужна для того, чтобы сохранять её в базу данных в виде текста,
    // а затем оттуда парсить.

    public Spell(string name, int level, string school, string source, 
        string castingTime, string range, string duration,
        IReadOnlyCollection<char> components, IReadOnlyCollection<string> materials,
        Classes classes, string descripton,
        bool concetration = false, bool ritual = false, DamageType damageType = DamageType.None,
        IReadOnlyCollection<string> archetypes = null, Func<int, int, Damage> damage = null)
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
        Concetration = concetration;
        Ritual = ritual;
        Classes = classes;
        Descripton = descripton;
        this.damageType = damageType;
        Archetypes = archetypes;
        this.damage = damage;
    }
    
    public Damage Damage(int characterLevel, int spellSlotLevel) 
        => damage(characterLevel, spellSlotLevel);
    // Соответственно, лямбда - это внутренняя информация. Пользователям можно дать красивый API к ней.

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