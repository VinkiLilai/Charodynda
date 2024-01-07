using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Charodynda.Infrastructure.Database;
using Newtonsoft.Json;

namespace Charodynda.Domain;

public class Character
{
    public Character()
    {
        spells = new HashSet<Spell>();
        levelsInClasses = new Dictionary<Class, int>();
    }

    public readonly int id;
    [JsonProperty("Name")]
    private string name;
    [JsonProperty("Spells")]
    private HashSet<Spell> spells;
    [JsonProperty("SpellSlots")]
    private Dictionary<int, LevelSpellSlots>? spellSlots;

    [JsonProperty("WarlockSpellSlots")] 
    public (int level, LevelSpellSlots spellSlots)? WarlockSpellSlots { get; set; }

    [JsonProperty("LevelsInClasses")]
    private Dictionary<Class, int> levelsInClasses;
    [JsonProperty("Intelligence")]
    private int intelligence;
    [JsonProperty("Wisdom")]
    private int wisdom;
    [JsonProperty("Charisma")]
    private int charisma;

    public IReadOnlyCollection<Spell> Spells => spells.OrderBy(spell => spell.Level).ToList();
    public IReadOnlyDictionary<int, LevelSpellSlots>? SpellSlots => spellSlots;
    public IReadOnlyDictionary<Class, int> LevelsInClasses => levelsInClasses;

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
            if (levelsInClasses.Count == 1 || WarlockSpellSlots.Value.level == level)
            {
                WarlockSpellSlots.Value.spellSlots.Count--;
                return;
            }
        if (!spellSlots.ContainsKey(level))
            throw new AggregateException("There's no spell slot this level to use");
        spellSlots[level].Count--;
    }

    public void SlotsUpdate()
    {
        if (this.HasWarlockClass())
            WarlockSpellSlots.Value.spellSlots.UpdateSpellSlots();
        if (this.HasNonWarlockClass())
            return;
        foreach (var slots in spellSlots.Values)
            slots.UpdateSpellSlots();
    }

    public void ChangeClassLevel(Class characterClass, int level)
    {
        if (level < 1)
            throw new ArgumentException("Level cannot be less than the first level");
        levelsInClasses[characterClass] = level;
        RecalculateSpellSlots();
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
        RecalculateSpellSlots();
    }

    public void RemoveClass(Class characterClass)
    {
        if (!levelsInClasses.ContainsKey(characterClass))
            return;
        levelsInClasses.Remove(characterClass);
        RecalculateSpellSlots();
    }

    private void RecalculateSpellSlots()
    {
        WarlockSpellSlots = this.GetInitialWarlockSpellSlots();
        spellSlots = this.GetInitialSpellSlots();
    }
    
}

public static class CharacterExtensions
{
    public static bool CheckCharacteristics(this int value) => value >= 0;

    public static Dictionary<int, LevelSpellSlots>? GetInitialSpellSlots(this Character character)
    {
        const string path = "../../../../Charodynda.Infrastructure/Database/Charodynda.db";
        var dbSpellSlots = new DatabaseApi<LevelSpellSlots[]>(path);
        if (!character.HasNonWarlockClass())
            return null;
        var totalCharacterLevel = CalculateTotalCharacterLevel(character.LevelsInClasses);
        return dbSpellSlots.GetById(totalCharacterLevel)
            .Select((x, i) => (x, i + 1))
            .ToDictionary(x => x.Item2, x => x.Item1);
    }

    public static (int level, LevelSpellSlots spellSlots)? GetInitialWarlockSpellSlots(this Character character)
    {
        // захардкодить

        if (!character.HasWarlockClass())
            return null;
        var warlockLevel = character.LevelsInClasses[Class.Warlock];
        throw new NotImplementedException();
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

    public static bool HasWarlockClass(this Character character) =>
        character.LevelsInClasses.ContainsKey(Class.Warlock);
    
    public static bool HasNonWarlockClass(this Character character) =>
        character.LevelsInClasses.Any(pair => pair.Key != Class.Warlock);
}