using System;

namespace SimpCity {
    class Program {
        static void Main(string[] args) {
            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction((m) => Console.WriteLine("Welcome, mayor of Simp City!\n" + new string('=', 26)))
                .AddOption("Start new game", (m) => {
                    Console.WriteLine("TODO new game");
                    })
                .AddOption("Load saved game", (m) => {
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
