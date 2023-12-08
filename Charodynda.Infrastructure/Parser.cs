using HtmlAgilityPack;
using System.Net;

namespace Charodynda.Infrastructure;

public class Parser
{
    public Dictionary<string, string> ParseOneSpellPage(string link)
    {
        var page = GetHtml(link);
        var spell = new Dictionary<string, string>
        {
            {"Name", page.DocumentNode.SelectSingleNode("//h2[@class='card-title']").ChildNodes[0].InnerHtml}
        };
        
        var spellParams = page.DocumentNode.SelectSingleNode("//ul[@class='params']").ChildNodes;
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