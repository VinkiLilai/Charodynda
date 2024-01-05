using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Charodynda.Domain;

public class Character
{
    public Character(string name, HashSet<Spell> spells, List<int> spellSlots, Dictionary<Class, int> levelsInClasses,
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
        spellSlots = this.InitSpellSlots();
    }

    [JsonProperty("Name")]
    private string name;
    [JsonProperty("SpellIds")]
    private HashSet<int> spellIds;
    private HashSet<Spell> spells;
    [JsonProperty("SpellSlots")]
    private Dictionary<int, LevelSpellSlots> spellSlots;
    [JsonProperty("WarlockSpellSlots")]
    private (int level, LevelSpellSlots spellSlots) warlockSpellSlots;
    [JsonProperty("LevelsInClasses")]
    private Dictionary<Class, int> levelsInClasses;
    [JsonProperty("Intelligence")]
    private int intelligence;
    [JsonProperty("Wisdom")]
    private int wisdom;
    [JsonProperty("Charisma")]
    private int charisma;

    public IReadOnlyCollection<Spell> Spells => spells.OrderBy(spell => spell.Level).ToList();
    public IReadOnlyCollection<int> SpellSlots => spellSlots;
    public IReadOnlyDictionary<Class, int> LevelsInClasses;

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

    public void ChangeClassLevel(Class characterClass, int level)
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

    public void AddClass(Class characterClass)
    {
        if (!levelsInClasses.ContainsKey(characterClass))
            levelsInClasses.Add(characterClass, 1);
    }
    
    public void RemoveClass(Class characterClass)
    {
        if (levelsInClasses.ContainsKey(characterClass))
            levelsInClasses.Remove(characterClass);
    }
}

internal static class CharacterExtensions
{
    public static bool CheckCharacteristics(this int value) => value >= 0;
}