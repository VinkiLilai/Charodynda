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

        Console.WriteLine(
            string.Join("\r\n", 
                dbSpellSlots.GetAll().Select(
                    x => string.Join(" ", x.Select(slot => slot.MaxCount.ToString()))
                    )
                )
            );

        // Holy shit, оно работает
    }

    private static LevelSpellSlots[] MakeSpellSlots(params int[] maxCounts)
    {
        return maxCounts.Select(count => new LevelSpellSlots(count)).ToArray();
    }
    
    //public static Spell Parse(Dictionary<string, string> spellDict)
    //{
     //   return new Spell();
    //}
}