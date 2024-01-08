using Charodynda.Domain;
using Charodynda.Infrastructure;
using Charodynda.Infrastructure.Database;
using Newtonsoft.Json;

namespace Charodynda.ConsoleTesting;

public static class Program
{
    /*private static Dictionary<string, string> sourcesFixed = new()
    {
        { "«Icewind Dale" , "«Icewind Dale: Rime of the Frostmaiden»" },
        { "«Spelljammer" , "«Spelljammer: Adventures in Space»" },
        { "«Strixhaven" , "«Strixhaven: A Curriculum of Chaos»" },
        { "«Planescape" , "«Planescape: Adventures in the Multiverse»" }
    };*/ // this emerged due to a bug in Parser.cs 
    
    static void Main()
    {
        // nothing already :)
    }

    /*private static void FillDBwithSpells()
    {
        using var dbSpells = new DatabaseApi<Spell>("../../../../Charodynda.Infrastructure/Database/Charodynda.db");
        // путь до базы данных из Charodynda/Charodynda.ConsoleTesting/bin/net6/
        // в целом, до бд можно дойти любым способом, главное знать путь до папки солюшена
        
        var spells = new HashSet<Spell>();
        foreach (var link in Parser.GetAllSpellLinks())
        {
            Dictionary<string, string> spellDict;
            while (true)
            {
                try
                {
                    spellDict = Parser.ParseOneSpellPage(link);
                    break;
                }
                catch
                {
                    // ignored
                }
            }

            spells.Add(ParseSpell(spellDict));
            Console.Write("\r" + spells.Count);
        }

        foreach (var spell in spells)
        {
            dbSpells.Add(spell);
        }
    }*/

    /*private static Spell ParseSpell(Dictionary<string, string> spellDict)
    {
        var name = spellDict["Name"];
        var level = int.Parse(spellDict["Level"].Contains('З') ? "0" : spellDict["Level"]);
        var schoolSplit = spellDict["SpellSchool"].Split();
        var school = schoolSplit[0];
        var ritual = schoolSplit.Length > 1;
        var source = spellDict["Source"];
        source = sourcesFixed.GetValueOrDefault(source, source);
        var castingTime = spellDict["CastingTime"];
        var range = spellDict["Range"];
        var duration = spellDict["Duration"];
        var concentration = duration.Contains("Концентрация");
        var componentsSplit = spellDict["Components"].Split('(');
        var components = componentsSplit[0].Trim();
        var materials = componentsSplit.Length > 1 ? componentsSplit[1].Split(")")[0] : "";
        var archetypesString = spellDict.GetValueOrDefault("Archetypes", "");
        var classString = spellDict.GetValueOrDefault("Classes", "");
        var classDesc = ParseClass(classString, archetypesString);
        var archetypes = archetypesString.Length > 0 ? archetypesString.Split(", ") : Array.Empty<string>();
        var description = spellDict["Description"];
        
        return new Spell(name, level, school, source, castingTime, range, duration, components, materials, 
            concentration, ritual, classDesc, archetypes, description);
    }*/

    private static Class ParseClass(string classDescStr, string archetypes)
    {
        var classDesc = Class.None;

        if (classDescStr.Contains("изобретатель"))
            classDesc |= Class.Artificer;
        if (classDescStr.Contains("колдун"))
            classDesc |= Class.Warlock;
        if (classDescStr.Contains("бард"))
            classDesc |= Class.Bard;
        if (classDescStr.Contains("жрец"))
            classDesc |= Class.Cleric;
        if (classDescStr.Contains("друид"))
            classDesc |= Class.Druid;
        if (classDescStr.Contains("волшебник"))
            classDesc |= Class.Wizard;
        if (classDescStr.Contains("паладин"))
            classDesc |= Class.Paladin;
        if (classDescStr.Contains("следопыт"))
            classDesc |= Class.Ranger;
        if (archetypes.Contains("мистический ловкач"))
            classDesc |= Class.RogueArcaneTrickster;
        if (classDescStr.Contains("чародей"))
            classDesc |= Class.Sorcerer;
        
        return classDesc;
    }
}