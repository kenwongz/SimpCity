using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SimpCity {
    [ExcludeFromCodeCoverage]
    class Program {
        static void Main(string[] args) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            bool isDebugMode = false;
            Game makeGameFunc() => new Game(!isDebugMode ? null : new GameOptions {
                DisableAdjacentRule = true,
                AllowAllBuildingTypes = true,
            });

            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction((m) => {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine($"   SimpCity v{informationVersion}   ");
                    Console.ResetColor();

                    // Notify if the session is in debug
                    if (isDebugMode) {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine($"   (Debug mode)   ");
                        Console.ResetColor();
                    }

                    Console.WriteLine("Welcome, mayor of Simp City!\n" + new string('=', 28));
                })
                .AddOption("Start new game", (m) => {
                    Game game = makeGameFunc();
                    game.Play();
                })
                .AddOption("Load saved game", (m) => {
                    Game game = makeGameFunc();
                    game.Restore();
                    game.Play();
                })
                .AddHeading()
                .AddOption("Toggle debug mode", (m) => {
                    isDebugMode = !isDebugMode;
                    Console.Write("Debug mode is now: ");
                    if (isDebugMode) {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write("on");
                        Console.ResetColor();
                    } else {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write("off");
                        Console.ResetColor();
                    }
                    Console.WriteLine();
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
