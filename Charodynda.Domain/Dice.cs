using System;

namespace Charodynda.Domain;

// Причина отделения кубиков от урона - в том, что, собственно,
// кубики и урон разные сущности. Урон - чуть более узкая, так как
// это какой-то конкретный тип урона в совокупности с каким-то количеством,
// выражаемым кубиками.
// Также имело смысл сделать это для реализации Average и Throw() для 
// логики взятия среднего и собственно броска кубиков.

public struct Dice
{
    private static readonly Random rng = new(); 
    
    public int Amount { get; }
    public DieSize Size { get; }
    
    public int Average => ((int)Size + 1) * (Amount / 2) + Amount % 2 * (int)Size / 2;

    public Dice(int amount, DieSize size)
    {
        Amount = amount;
        Size = size;
    }
    
    public int Roll()
    {
        var result = 0;
        for (var i = 0; i < Amount; i++) 
            result += rng.Next(1, (int)Size);
        return result;
    }

    public override string ToString()
    {
        return $"{Amount}к{(int)Size}";
    }
}

public enum DieSize
{
    D4 = 4,
    D6 = 6,
    D8 = 8,
    D10 = 10, 
    D12 = 12,
    D20 = 20,
    D100 = 100
}