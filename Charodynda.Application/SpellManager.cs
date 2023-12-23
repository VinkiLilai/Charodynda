using System.Data.Entity;
using Charodynda.Domain;
using Charodynda.Domain.Zaglushki;
using Charodynda.Infrastructure.Database;

namespace Charodynda.Application;

public static class SpellManager
{
    static SpellManager()
    {
        var serializer = new SpellSerializer();
        dbSpells = new DatabaseApi<Spell>("Charodynda.db", serializer);
    }

    private static DatabaseApi<Spell> dbSpells;

    public static IEnumerable<Spell> GetAllSpells() => dbSpells.GetAll("Spells");

    public static IEnumerable<Spell> GetSpellsByFilter(IFilter<Spell> filter) => dbSpells
        .FindIdsByFilter("Spells", filter)
        .Select(id => dbSpells.FindById("Spells", id));
}