using System.Runtime.Serialization;
using Charodynda.Infrastructure.Database;

namespace Charodynda.Domain.Zaglushki;

public class CharacterSerializer: IObjectSerializer<Character>
{
    public Character Serialize(Dictionary<string, object> properties)
    {
        throw new NotImplementedException();
    }
}