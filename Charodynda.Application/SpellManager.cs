using System.Data.Entity;
using Charodynda.Domain;
using Charodynda.Infrastructure.Database;

namespace Charodynda.Application;

public static class SpellManager
{
    static SpellManager()
    {
        const string path = "../../../../Charodynda.Infrastructure/Database/Charodynda.db";
        dbSpells = new DatabaseApi<Spell>(path);
    }

    private static DatabaseApi<Spell> dbSpells;

    public static IEnumerable<Spell> GetAllSpells() => dbSpells.GetAll();

    public static IEnumerable<Spell> GetSpellsByFilter(Dictionary<string, object[]> filter) => dbSpells
        .GetByFilter(filter);
}