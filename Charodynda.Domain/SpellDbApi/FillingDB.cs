using Charodynda.Infrastructure.Database;
using Charodynda.Infrastructure;
using HtmlAgilityPack;
using System.Data.SQLite;

namespace Charodynda.Domain;

public static class FillingDB
{
    public static void ParseAndAddSpells()
    {
        var spellSerializer = new SpellSerialiser();
        var db = new DatabaseApi<Spell>("Charodynda.db", spellSerializer);

        foreach (var spellDict in Parser.ParseSpells())
        {
            //var spell = spellSerializer.Serialize(spellDict);
            //db.Add(spell);
        }
    }
}