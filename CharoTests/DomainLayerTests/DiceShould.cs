using Charodynda.Domain;

namespace CharodyndaApp.Tests;

[TestFixture]
public class DiceShould
{
    [TestCase(1, DieSize.D6, 3)]
    [TestCase(2, DieSize.D6, 7)]
    [TestCase(3, DieSize.D6, 10)]
    public void TestAverage(int amount, DieSize size, int expected)
    {
        var dice = new Dice(amount, size);
        Assert.That(expected, Is.EqualTo(dice.Average));
    }
    
    [TestCase(1, DieSize.D6, "1к6")]
    [TestCase(1, DieSize.D20, "1к20")]
    [TestCase(8, DieSize.D6, "8к6")]
    [TestCase(2, DieSize.D10, "2к10")]
    public void TestToString(int amount, DieSize size, string expected)
    {
        var dice = new Dice(amount, size);
        Assert.That(expected, Is.EqualTo(dice.ToString()));
    }
}