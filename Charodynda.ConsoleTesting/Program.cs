using System.Data;
using System.Data.SQLite;
using Charodynda.Domain;
using Charodynda.Infrastructure;
using Charodynda.Infrastructure.Database;

namespace Charodynda.ConsoleTesting;

public static class Program
{
    static void Main()
    {
        using var dbSpellSlots = new DatabaseApi<LevelSpellSlots[]>("../../../../Charodynda.Infrastructure/Database/Charodynda.db");
        // путь до базы данных из Charodynda/Charodynda.ConsoleTesting/bin/net6/
        // в целом, до бд можно дойти любым способом, главное знать путь до папки солюшена


    }
}