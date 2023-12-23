using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Charodynda.Domain;

public class Character
{
    public Character(string name, HashSet<Spell> spells, List<int> spellSlots, Dictionary<Classes, int> levelsInClasses,
        int intelligence, int wisdom, int charisma)
    {
        this.spells = spells;
        this.spellSlots = spellSlots;
        this.intelligence = intelligence;
        this.levelsInClasses = levelsInClasses;
        this.wisdom = wisdom;
        this.charisma = charisma;
    }

    public Character()
    {
        spells = new HashSet<Spell>();
        spellSlots = new List<int>();
    }

    private string name;
    private HashSet<Spell> spells;
    private List<int> spellSlots;
    private Dictionary<Classes, int> levelsInClasses;
    private int intelligence;
    private int wisdom;
    private int charisma;

    public IReadOnlyCollection<Spell> Spells => spells.OrderBy(spell => spell.Level).ToList();
    public IReadOnlyCollection<int> SpellSlots => spellSlots;
    public IReadOnlyDictionary<Classes, int> LevelsInClasses;

    public string Name
    {
        get => name;
        set => name = value == string.Empty ? "NoName" : value; 
    }

    // Логика - возможность мультикласса, а также автоматический расчёт количества ячеек
    // (тот, кто им займётся, возненавидит колдуна)

    public int Intelligence
    {
        get => intelligence;
        set
        {
            if (value.CheckCharacteristics()) intelligence = value;
        }
    }

    public int Wisdom
    {
        get => wisdom;
        set
        {
            if (value.CheckCharacteristics()) wisdom = value;
        }
    }

    public int Charisma
    {
        get => charisma;
        set
        {
            if (value.CheckCharacteristics()) charisma = value;
        }
    }

    public void SpellUsage() //TODO: реализовать применение заклинания: вычесть ячейку если требуется, вывести уровень
    {
        throw new NotImplementedException();
    }

    public void SlotsUpdate() //TODO: реализовать восстановление ячеек, например, при отдыхе
    {
        throw new NotImplementedException();
    }

    public void ChangeClassLevel(Classes characterClass, int level)
    {
        if (level < 1)
            throw new ArgumentException("Level cannot be less than the first level");
        levelsInClasses[characterClass] = level;
        //TODO: Поменять набор ячеек заклинания в соответствии с новым уровнем
        throw new NotImplementedException();
    }

    public void AddSpell(Spell spell)
    {
        spells.Add(spell);
    }

    public void RemoveSpell(Spell spell)
    {
        spells.Remove(spell);
    }

    public void AddClass(Classes characterClass)
    {
        if (!levelsInClasses.ContainsKey(characterClass))
            levelsInClasses.Add(characterClass, 1);
    }
    
    public void RemoveClass(Classes characterClass)
    {
        if (levelsInClasses.ContainsKey(characterClass))
            levelsInClasses.Remove(characterClass);
    }
}

internal static class CharacterExtensions
{
    public static bool CheckCharacteristics(this int value) => value >= 0;
}