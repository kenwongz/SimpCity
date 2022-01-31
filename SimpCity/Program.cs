using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SimpCity {
    [ExcludeFromCodeCoverage]
    class ProgramSettings {
        public bool IsDebugMode { get; set; } = false;
        public int GridWidth { get; set; } = 4;
        public int GridHeight { get; set; } = 4;
    }

    [ExcludeFromCodeCoverage]
    class Program {
        static void Main(string[] args) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            // Create data folder

            ProgramSettings pSettings = new ProgramSettings();
            GlobalLeaderboard glb = new GlobalLeaderboard(DataPaths.LeaderboardFile);

            // Helper anonymous function to create game
            Game makeGameFunc() => new(!pSettings.IsDebugMode ? null : new GameOptions {
                DisableAdjacentRule = true,
                AllowAllBuildingTypes = true,
            });

            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction((m) => {
                    Utils.WriteLineColored($"   SimpCity v{informationVersion}   ",
                        foreground: ConsoleColor.Black,
                        background: ConsoleColor.Green);

                    // Notify if the session is in debug
                    if (pSettings.IsDebugMode) {
                        Utils.WriteLineColored($"   (Debug mode)   ",
                            foreground: ConsoleColor.Black,
                            background: ConsoleColor.Red);
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
                .AddOption("Show high scores", (m) => {
                    Leaderboard lb = glb.GetLeaderboard(pSettings.GridWidth, pSettings.GridHeight);
                    lb.Display();
                })
                .AddHeading()
                .AddOption("Toggle debug mode", (m) => {
                    pSettings.IsDebugMode = !pSettings.IsDebugMode;
                    Console.Write("Debug mode is now: ");
                    if (pSettings.IsDebugMode) {
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
