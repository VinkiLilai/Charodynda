namespace Charodynda.Infrastructure.Database;

public interface IDatabaseApi<T>
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    IEnumerable<T> GetByFilter(Dictionary<string, object[]> filterValues);
    void Update(int id, T newObj);
    void RemoveById(int id);
    void Add(T obj);
}
