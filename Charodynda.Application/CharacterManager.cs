using Charodynda.Domain;
using Charodynda.Infrastructure.Database;

namespace Charodynda.Application;

public static class CharacterManager
{
    static CharacterManager()
    {
        const string path = "../../../../Charodynda.Infrastructure/Database/Charodynda.db";
        dbCharacter = new DatabaseApi<Character>(path);
    }

    private static DatabaseApi<Character> dbCharacter;

    public static IEnumerable<Character> GetAllCharacters() => dbCharacter.GetAll();

    public static IEnumerable<Spell> GetCharacterSpells(Character character) => character.Spells;
    
    public static Character CreateNewCharacter()
    {
        var newCharacter = new Character();
        dbCharacter.Add(newCharacter);
        return newCharacter;
    }

    public static void ChangeCasterCharacteristics(this Character character, int intelligenceValue, int wisdomValue,
        int charismaValue)
    {
        character.Intelligence = intelligenceValue;
        character.Charisma = charismaValue;
        character.Wisdom = wisdomValue;
        dbCharacter.Update(character.id, character);
    }

    public static string ChangeCharacterName(this Character character, string name)
    {
        character.Name = name;
        dbCharacter.Update(character.id, character);
        return character.Name;
    }

    public static IEnumerable<Class> AddCharacterClass(this Character character, Class characterClass)
    {
        character.AddClass(characterClass);
        dbCharacter.Update(character.id, character);
        return character.LevelsInClasses.Keys;
    }

    public static IEnumerable<Class> RemoveCharacterClass(this Character character, Class characterClass)
    {
        character.RemoveClass(characterClass);
        dbCharacter.Update(character.id, character);
        return character.LevelsInClasses.Keys;
    }

    public static (Class characterClass, int level) ChangeClassLevel(this Character character, Class characterClass,
        int level)
    {
        character.ChangeClassLevel(characterClass, level);
        dbCharacter.Update(character.id, character);
        return ValueTuple.Create(characterClass, character.LevelsInClasses[characterClass]);
    }

    public static void ChangeSpellSlotCount(this Character character, int spellSlotLevel, int newCount)
    {
        character.SpellSlots[spellSlotLevel].Count = newCount;
        dbCharacter.Update(character.id, character);
    }
    
    public static void ChangeWarlockSpellSlotCount(this Character character, int newCount)
    {
        character.WarlockSpellSlots.Value.spellSlots.Count = newCount;
        dbCharacter.Update(character.id, character);
    }

    public static IEnumerable<int> GetAvailableCharacterSpellSlotsLevels(this Character character)
    {
        var spellSlotsLevels = new HashSet<int>();
        if (character.HasNonWarlockClass())
            spellSlotsLevels = character.SpellSlots
                .Where(slot => slot.Value.Count != 0)
                .Select(slot => slot.Key)
                .ToHashSet();
        if (character.HasWarlockClass())
            spellSlotsLevels.Add(character.WarlockSpellSlots.Value.level);
        return spellSlotsLevels.OrderBy(x => x).ToList();
    }

    public static IEnumerable<int> GetAvailableSpellCastSpellSlotsLevels(this Character character, Spell spell) =>
        character
            .GetAvailableCharacterSpellSlotsLevels()
            .Where(level => level >= spell.Level);
}