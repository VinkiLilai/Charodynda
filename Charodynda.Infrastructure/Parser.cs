using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Charodynda.Infrastructure;

public static class Parser
{
    public static IEnumerable<Dictionary<string, string>> ParseSpells()
        => GetAllSpellLinks()
            .Select(ParseOneSpellPage);

    private static IEnumerable<string> GetAllSpellLinks()
    {
        var doc = GetHtml("https://dnd.su/spells/");
        var linkListNode = doc.DocumentNode.SelectNodes("//script").First(node => node.InnerHtml.Contains("Fireball"));
        var linkListNodeText = linkListNode.InnerText;
        
        const string pattern = "\"link\":\"(.*?)\"";
        foreach (Match match in Regex.Matches(linkListNodeText, pattern))
        {
            var link = match.Groups[1].Value.Replace("\\/", "/");
            yield return "https://dnd.su" + link;
        }
    }

    public static Dictionary<string, string> ParseOneSpellPage(string link)
    {
        var page = GetHtml(link);
        var spell = new Dictionary<string, string>
        {
            {"Name", page.DocumentNode.SelectSingleNode("//h2[@class='card-title']").ChildNodes[0].InnerHtml}
        };

        var spellParams = page.DocumentNode.SelectSingleNode("//ul[@class='params card__article-body']").ChildNodes;
        var spellParamsNameDict = GetParamsNameDict();

        foreach (var node in spellParams)
        {
            if (node.Name != "li")
                continue;
            if (node.Attributes.Contains("class"))
            {
                // parsing class = size-type-alignment and class = subsection desc here
                if (node.HasClass("size-type-alignment"))
                {
                    var (level, spellSchool) = ParseSizeTypeAlignment(node);
                    spell.Add("Level", level);
                    spell.Add("SpellSchool", spellSchool);
                }
                else
                {
                    var desc = node.InnerText;
                    spell.Add("Description", desc);
                }
                continue;
            }
            var paramInfo = node.InnerText.Split(':').Select(s => s.Trim()).ToList();
            if (!spellParamsNameDict.ContainsKey(paramInfo[0]))
                continue;
            spell[spellParamsNameDict[paramInfo[0]]] = paramInfo[1];
        }

        return spell;
    }

    private static (string level, string spellSchool) ParseSizeTypeAlignment(HtmlNode node)
    {
        var info = node.InnerText.Split(",").Select(s => s.Trim()).ToList();
        return ValueTuple.Create(info[0].FirstOrDefault().ToString(), info[1]);
    }

    private static HtmlDocument GetHtml(string link) => new HtmlWeb().Load(link);

    private static Dictionary<string, string> GetParamsNameDict() => new()
    {
        {"Время накладывания", "CastingTime"},
        {"Дистанция", "Range"},
        {"Компоненты", "Components"},
        {"Длительность", "Duration"},
        {"Классы", "Classes"},
        {"Архетипы", "Archetypes"},
        {"Источник", "Source"},
    };
}