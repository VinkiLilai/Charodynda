using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Charodynda.Infrastructure.Database;
using Newtonsoft.Json;

namespace Charodynda.Domain;

public class Character
{
    public Character(string name, HashSet<Spell> spells, List<int> spellSlots, Dictionary<Class, int> levelsInClasses,
        int intelligence, int wisdom, int charisma)
    {
        this.spells = spells;
        //this.spellSlots = spellSlots;
        this.intelligence = intelligence;
        this.levelsInClasses = levelsInClasses;
        this.wisdom = wisdom;
        this.charisma = charisma;
    }

    public Character()
    {
        spells = new HashSet<Spell>();
        //spellSlots = this.InitSpellSlots();
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
    public IReadOnlyDictionary<Class, int> LevelsInClasses => levelsInClasses;

    public Character()
    {
        spells = new HashSet<Spell>();
    }

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

    public void SpellUsage(int level)
    {
        if (levelsInClasses.ContainsKey(Class.Warlock))
            if (levelsInClasses.Count == 1 || warlockSpellSlots.level == level)
            {
                warlockSpellSlots.spellSlots.Count--;
                return;
            }
        if (!spellSlots.ContainsKey(level))
            throw new AggregateException("There's no spell slot this level to use");
        spellSlots[level].Count--;
    }

    public void SlotsUpdate()
    {
        if (levelsInClasses.ContainsKey(Class.Warlock))
            warlockSpellSlots.spellSlots.UpdateSpellSlots();
        if (levelsInClasses.All(pair => pair.Key == Class.Warlock))
            return;
        foreach (var slots in spellSlots.Values)
            slots.UpdateSpellSlots();
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
        if (levelsInClasses.ContainsKey(characterClass))
            return;
        levelsInClasses.Add(characterClass, 1);
        if (characterClass == Class.Warlock)
            warlockSpellSlots = this.GetInitialWarlockSpellSlots();
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

    public static Dictionary<int, LevelSpellSlots> GetInitialSpellSlots(this Character character)
    {
        var dbSpellSlots = new DatabaseApi<Dictionary<int, LevelSpellSlots>>("SpellSlots.db");
        var totalCharacterLevel = CalculateTotalCharacterLevel(character.LevelsInClasses);
        return dbSpellSlots.FindById("SpellSlots", totalCharacterLevel);
    }

    public static (int level, LevelSpellSlots spellSlots) GetInitialWarlockSpellSlots(this Character character)
    {
        var dbSpellSlots = new DatabaseApi<(int level, LevelSpellSlots spellSlots)>("SpellSlots.db");
        var warlockLevel = character.LevelsInClasses[Class.Warlock];
        return dbSpellSlots.FindById("SpellSlots", warlockLevel);
    }

    public static int CalculateTotalCharacterLevel(IReadOnlyDictionary<Class, int> levelsInClasses)
    {
        var totalLevel = 0;
        var halfLevelClasses = new HashSet<Class>() {Class.Paladin, Class.Ranger};
        var oneThirdLevelClasses = new HashSet<Class>() {Class.FighterEldritchKnight, Class.RogueArcaneTrickster};
        foreach (var pair in levelsInClasses)
        {
            if (halfLevelClasses.Contains(pair.Key))
                totalLevel += (int) Math.Floor((double) pair.Value / 2);
            else if (oneThirdLevelClasses.Contains(pair.Key))
                totalLevel += (int) Math.Floor((double) pair.Value / 3);
            else if (pair.Key is Class.Artificer)
                totalLevel += (int) Math.Ceiling((double) pair.Value / 2);
            else if (pair.Key != Class.Warlock)
                totalLevel += pair.Value;
        }
        return totalLevel;
    }
}