namespace Charodynda.Domain;

public class Character
{
    public Character(IReadOnlyCollection<Spell> spells, IReadOnlyCollection<int> spellSlots, string intelligence, 
        string wisdom, string charisma)
    {
        Spells = spells;
        SpellSlots = spellSlots;
        Intelligence = intelligence;
        Wisdom = wisdom;
        Charisma = charisma;
    }
    
    public IReadOnlyCollection<Spell> Spells;
    public IReadOnlyCollection<int> SpellSlots;
    public string Intelligence { get; }
    public string Wisdom { get; }
    public string Charisma { get; }
    
    public void SpellUsage(){} //TODO: реализовать применение заклинания: вычесть ячейку если требуется, вывести уровень

    public void SlotsUpdate(){} //TODO: реализовать восстановление ячеек, например, при отдыхе
}