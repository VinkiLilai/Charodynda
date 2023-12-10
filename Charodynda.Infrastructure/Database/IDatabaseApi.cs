namespace Charodynda.Infrastructure.Database;

public interface IDatabaseApi<T>
{
    T FindById(string table, int id);
    IEnumerable<int> FindIdsByFilter(string table, IFilter<T> filter);
    
    void RemoveById(string table, int id);
    void Add(string table, T obj);
}