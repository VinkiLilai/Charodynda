namespace Charodynda.Domain;

public class LevelSpellSlots
{
    public LevelSpellSlots(int maxCount)
    {
        this.maxCount = maxCount;
        count = maxCount;
    }

    private int count;
    private int maxCount;

    public int MaxCount
    {
        get => maxCount;
        set
        {
            if (value is < 0 or > 4)
                throw new ArgumentException($"Max count cannot be less than zero and more than four");
            maxCount = value;
        }
    }

    public int Count
    {
        get => count;
        set
        {
            if (value < 0 || value > maxCount)
                throw new InvalidOperationException($"Slots count cannot be less than zero and more than {maxCount}");
            count = value;
        }
    }

    public void UpdateSpellSlots() => count = maxCount;
}