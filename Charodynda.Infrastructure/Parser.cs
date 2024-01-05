using HtmlAgilityPack;
using System.Net;

namespace Charodynda.Infrastructure;

public class Parser
{
    public string ParseOnePage(string link)
    {
        var page = GetHtml(link);
        var spellName = page.DocumentNode.SelectSingleNode("//h2[@class='card-title']").ChildNodes[0].InnerHtml;
        return spellName;
    }
    public HtmlDocument GetHtml(string link) => new HtmlWeb().Load(link);
}
