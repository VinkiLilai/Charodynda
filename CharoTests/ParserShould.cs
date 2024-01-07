using Charodynda.Infrastructure;

namespace CharodyndaApp.Tests;

[TestFixture]
public class ParserShould
{
    [Test]
    public void ParserCanReadSpellName()
    {
        var spell = Parser.ParseOneSpellPage("https://dnd.su/spells/205-fireball/");
        Assert.That("Огненный шар [Fireball]", Is.EqualTo(spell["Name"]));
    }

    [Test]
    public void ParserCanReadSpellParams()
    {
        var spell = Parser.ParseOneSpellPage("https://dnd.su/spells/205-fireball/");
        Assert.AreEqual(spell["CastingTime"],"1 действие");
    }
    
    [Test]
    public void ParserCanReadAllSpells()
    {
        Assert.AreEqual(Parser.ParseSpells().FirstOrDefault()["Name"], "Адское возмездие [Hellish rebuke]");
    }
}