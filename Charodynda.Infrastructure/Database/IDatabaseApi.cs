namespace Charodynda.Infrastructure.Database;

public interface IDatabaseApi<T>
{
    IEnumerable<T> GetAll(string table);
    T FindById(string table, int id);
    IEnumerable<T> FindByFilter(string table, Dictionary<string, object[]> filterValues);
    void Update(string table, int id, T newObj);
    void RemoveById(string table, int id);
    void Add(string table, T obj);
}
