namespace Charodynda.Infrastructure.Database;

public interface IObjectSerializer<out T>
{
    T Serialize(Dictionary<string, object> properties);
}