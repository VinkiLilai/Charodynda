using Charodynda.Infrastructure;
using NUnit.Framework;

namespace CharodyndaApp.Infrastructure;

[TestFixture]
public class ParserShould
{
    [Test]
    public void ParserCanReadSpellName()
    {
        var parser = new Parser();
        var spellName = parser.ParseOnePage("https://dnd.su/spells/205-fireball/");
        Assert.Equals(spellName, "Огненный шар [Fireball]");
    }
}