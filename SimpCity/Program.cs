using System;

namespace SimpCity {
    class Program {
        static void Main(string[] args) {
            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction(() => Console.WriteLine("Welcome, mayor of Simp City!\n" + new string('=', 26)))
                .AddOption("Start new game", (_cmd) => {
                    Console.WriteLine("TODO new game");
                    })
                .AddOption("Load saved game", (_cmd) => {
                    Console.WriteLine("TODO load game");
                })
                .AddHeading()
                .AddExitOption("Exit");

            menu.DisplayInteraction();

            // Don't close automatically
            Console.WriteLine("Until next time, mayor! Press any key to exit the program.");
            Console.ReadKey();
        }
    }
}
