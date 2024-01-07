using System.Runtime.Serialization;
using Charodynda.Infrastructure.Database;

namespace Charodynda.Domain.Zaglushki;

public class SpellSerializer: IObjectSerializer<Spell>
{
    public Spell Serialize(Dictionary<string, object> properties)
    {
        throw new NotImplementedException();
    }
}