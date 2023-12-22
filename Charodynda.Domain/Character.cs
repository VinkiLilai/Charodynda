using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Charodynda.Domain;

public class Character
{
    public Character(HashSet<Spell> spells, List<int> spellSlots, int level, int intelligence,
        int wisdom, int charisma)
    {
        this.spells = spells;
        this.spellSlots = spellSlots;
        Level = level;
        this.intelligence = intelligence;
        this.wisdom = wisdom;
        this.charisma = charisma;
    }
    
    public Character()
    {
        spells = new HashSet<Spell>();
        spellSlots = new List<int>();
    }

    private HashSet<Spell> spells;
    private List<int> spellSlots;
    private int intelligence;
    private int wisdom;
    private int charisma;
    
    public int Level { get; private set; }
    
    public IReadOnlyCollection<Spell> Spells => spells.OrderBy(spell => spell.Level).ToList();
    public IReadOnlyCollection<int> SpellSlots => spellSlots;
    public IReadOnlyDictionary<Classes, int> LevelsInClasses;

    // Логика - возможность мультикласса, а также автоматический расчёт количества ячеек
    // (тот, кто им займётся, возненавидит колдуна)

    public int Intelligence { 
        get => intelligence;
        set { if (value.CheckCharacteristics()) intelligence = value; }
    }
    public int Wisdom { 
        get => wisdom;
        set { if (value.CheckCharacteristics()) wisdom = value; }
    }
    public int Charisma { 
        get => charisma;
        set { if (value.CheckCharacteristics()) charisma = value; }
    }

    public void SpellUsage() //TODO: реализовать применение заклинания: вычесть ячейку если требуется, вывести уровень
    {
        throw new NotImplementedException();
    }

    public void SlotsUpdate() //TODO: реализовать восстановление ячеек, например, при отдыхе
    {
        throw new NotImplementedException();
    }

    public void LevelUp() //TODO: обновить количество ячеек заклинаний, повысить сам уровень заклинателя
    {
        throw new NotImplementedException();
    }
    
    public void LevelUp(int level) //TODO: LevelUp но сразу до определенного уровня
    {
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
}

internal static class CharacterExtensions
{
    public static bool CheckCharacteristics(this int value) => value >= 0;
}
