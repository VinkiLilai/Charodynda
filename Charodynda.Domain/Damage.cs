namespace Charodynda.Domain;

public struct Damage
{
    public Damage(string type, IReadOnlyCollection<string> damagePerLvl)
    {
        Type = type;
        DamagePerLvl = damagePerLvl;
    }
    public string Type { get; }
    public IReadOnlyCollection<string> DamagePerLvl;
}