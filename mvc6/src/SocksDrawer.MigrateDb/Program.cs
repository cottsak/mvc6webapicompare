using System;
using System.Linq;
using System.Reflection;
using DbUp;

namespace SocksDrawer.MigrateDb
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var connectionString =
                args.FirstOrDefault()
                ?? "Data Source=.\\sqlexpress;Initial Catalog=SocksDrawer.Mvc6;Integrated Security=True";

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
