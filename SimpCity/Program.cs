using System;
using System.Reflection;

namespace SimpCity {
    class Program {
        static void Main(string[] args) {
            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction((m) => Console.WriteLine("Welcome, mayor of Simp City!\n" + new string('=', 28)))
                .AddOption("Start new game", (m) => {
                    Game game = new Game();
                    game.Play();
                })
                .AddOption("Load saved game", (m) => {
                    Game game = new Game();
                    game.Restore();
                    game.Play();
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
