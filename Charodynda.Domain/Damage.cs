namespace Charodynda.Domain;

public struct Damage
{
    public DamageType Type { get; }
    public Dice Dice { get; }
}

public enum DamageType
{
    None,
    
    Bludgeoning, 
    Piercing,
    Slashing,
    
    Acid,
    Cold, 
    Fire,
    Lightning, 
    Poison, 
    Thunder, 
    
    Force, 
    Necrosis,
    Psychic,
    Radiant
}