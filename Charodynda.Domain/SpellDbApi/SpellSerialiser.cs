using Charodynda.Infrastructure.Database;

namespace Charodynda.Domain;

public class SpellSerialiser : IObjectSerializer<Spell>
{
    public Spell Serialize(Dictionary<string, object> properties)
    {
        throw new NotImplementedException();
    }

    public Spell Serialize(Dictionary<string, string> data)
    {
        string name = data["Name"];
        int level = int.Parse(data["Level"]);
        string school = data["School"];
        string source = data["Source"];
    
        string castingTime = data["CastingTime"];
        string range = data["Range"];
        string duration = data["Duration"];
    
        List<char> components = new List<char>(data["Components"]);
        List<string> materials = new List<string>(data["Materials"].Split(';'));
        bool concentration = bool.Parse(data["Concentration"]);
        bool ritual = bool.Parse(data["Ritual"]);
    
        Classes classes = (Classes)Enum.Parse(typeof(Classes), data["Classes"]);
        List<string> archetypes = new List<string>(data["Archetypes"].Split(';'));

        string description = data["Description"];

        DamageType damageType = (DamageType)Enum.Parse(typeof(DamageType), data["DamageType"]);

        return new Spell(name, level, school, source, castingTime, range, duration,
            components, materials, classes, description, concentration, ritual);
    }
}