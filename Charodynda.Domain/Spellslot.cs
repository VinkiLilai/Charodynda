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
            if (value is >= 0 and <= 4)
                maxCount = value;
        }
    }
    
    public int Count
    {
        get => count;
        set
        {
            if (value >= 0 && value <= maxCount)
                count = value;
        }
    }

    public void UpdateSpellSlots() => count = maxCount;
}