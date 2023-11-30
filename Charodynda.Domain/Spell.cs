using System;
using System.Reflection;

namespace Charodynda.Domain;

public class Spell
{
    public Spell(string name, int level, string spellSchool, string castingTime, string range, 
        IReadOnlyCollection<char> components, IReadOnlyCollection<string> materials, string duration, 
        IReadOnlyCollection<string> classes, string descripton, bool concetration = false)
    {
        Components = components;
        Materials = materials;
        Classes = classes;
        Name = name;
        Level = level;
        SpellSchool = spellSchool;
        CastingTime = castingTime;
        Range = range;
        Duration = duration;
        Descripton = descripton;
        Concetration = concetration;
    }

    public string Name { get; }
    public int Level { get; }
    public string SpellSchool { get; }
    public string CastingTime { get; }
    public string Range { get; }
    public IReadOnlyCollection<char> Components;
    public IReadOnlyCollection<string> Materials;
    public string Duration { get; }
    public IReadOnlyCollection<string> Classes;
    public string Descripton { get; }
    public bool Concetration { get; }


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