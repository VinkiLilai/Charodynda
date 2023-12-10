namespace Charodynda.Infrastructure.Database;

public interface IFilter<in T>
{
    void Configure(T obj);
    string ToSQLConstraints();
}