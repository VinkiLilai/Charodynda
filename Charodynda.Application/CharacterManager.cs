using Charodynda.Domain;
using Charodynda.Domain.Zaglushki;
using Charodynda.Infrastructure.Database;

namespace Charodynda.Application;

public static class CharacterManager
{
    static CharacterManager()
    {
        var serializer = new CharacterSerializer();
        dbSpells = new DatabaseApi<Character>("Charodynda.db", serializer);
    }

    private static DatabaseApi<Character> dbSpells;

    public static IEnumerable<Character> GetAllCharacters() => dbSpells.GetAll("Characters");

    public static IEnumerable<Spell> GetCharacterSpells(Character character) => character.Spells;
    
    public static Character CreateNewCharacter()
    {
        var newCharacter = new Character();
        //TODO: добавить персонажа в дата базу
        return newCharacter;
    }

    public static void ChangeCasterCharacteristics(Character character, int intelligenceValue, int wisdomValue,
        int charismaValue)
    {
        character.Intelligence = intelligenceValue;
        character.Charisma = charismaValue;
        character.Wisdom = wisdomValue;
        //TODO: обновить это добро в ДБ
    }

    public static string ChangeCharacterName(Character character, string name)
    {
        character.Name = name;
        //TODO: обновить это добро в ДБ
        return character.Name;
    }

    public static IEnumerable<Classes> AddCharacterClass(Character character, Classes characterClass)
    {
        character.AddClass(characterClass);
        //TODO: обновить это добро в ДБ
        return character.LevelsInClasses.Keys;
    }

    public static IEnumerable<Classes> RemoveCharacterClass(Character character, Classes characterClass)
    {
        character.RemoveClass(characterClass);
        //TODO: обновить это добро в ДБ
        return character.LevelsInClasses.Keys;
    }

    public static (Classes characterClass, int level) ChangeClassLevel(Character character, Classes characterClass,
        int level)
    {
        character.ChangeClassLevel(characterClass, level);
        //TODO: обновить это добро в ДБ
        return ValueTuple.Create(characterClass, character.LevelsInClasses[characterClass]);
    }
}