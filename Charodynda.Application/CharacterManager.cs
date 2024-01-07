using Charodynda.Domain;
using Charodynda.Domain.Zaglushki;
using Charodynda.Infrastructure.Database;

namespace Charodynda.Application;

public static class CharacterManager
{
    static CharacterManager()
    {
        dbCharacter = new DatabaseApi<Character>("Charodynda.db");
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

    public static void ChangeCasterCharacteristics(Character character, int intelligenceValue, int wisdomValue,
        int charismaValue)
    {
        character.Intelligence = intelligenceValue;
        character.Charisma = charismaValue;
        character.Wisdom = wisdomValue;
        dbCharacter.Update(id, character);
    }

    public static string ChangeCharacterName(Character character, string name)
    {
        character.Name = name;
        //TODO: обновить это добро в ДБ
        return character.Name;
    }

    public static IEnumerable<Class> AddCharacterClass(Character character, Class characterClass)
    {
        character.AddClass(characterClass);
        //TODO: обновить это добро в ДБ
        return character.LevelsInClasses.Keys;
    }

    public static IEnumerable<Class> RemoveCharacterClass(Character character, Class characterClass)
    {
        character.RemoveClass(characterClass);
        //TODO: обновить это добро в ДБ
        return character.LevelsInClasses.Keys;
    }

    public static (Class characterClass, int level) ChangeClassLevel(Character character, Class characterClass,
        int level)
    {
        character.ChangeClassLevel(characterClass, level);
        //TODO: обновить это добро в ДБ
        return ValueTuple.Create(characterClass, character.LevelsInClasses[characterClass]);
    }
}