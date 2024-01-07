using Charodynda.Domain;
using Charodynda.Infrastructure;
using Charodynda.Infrastructure.Database;

namespace Charodynda.ConsoleTesting;

public static class Program
{
    static void Main()
    {
        using var db = new DatabaseApi<Spell>("../../../../Charodynda.Infrastructure/Database/Charodynda.db");
        
    }

    //public static Spell Parse(Dictionary<string, string> spellDict)
    //{
     //   return new Spell();
    //}
}