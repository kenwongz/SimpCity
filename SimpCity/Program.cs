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
        const int MAX_DIMENSION = 10;

        /// <summary>
        /// Checks the score for leaderboard eligibility. If eligible, prompts for player's name to add
        /// to the leaderboard.
        /// </summary>
        /// <param name="game">The game that was just completed.</param>
        /// <param name="glb">The global leaderboard to add to.</param>
        static void DoLeaderboardEligibility(Game game, GlobalLeaderboard glb) {
            if (!game.HasEnded) return;

            int totalScore = 0;
            foreach (var entry in game.CalculateScores()) {
                int score = 0;
                foreach (int s in entry.Value) {
                    score += s;
                }
                totalScore += score;
            }

            Leaderboard lb = glb.GetLeaderboard(game.GridWidth, game.GridHeight);
            uint lbPosition = lb.ScorePointPosition(totalScore);
            if (lbPosition == 0) return;

            Console.WriteLine();
            Console.WriteLine($"Congratulations! You made the high score board at position {lbPosition}!");

            string name;
            do {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Please enter your name (max 20 chars): ");
                Console.ResetColor();

                name = Console.ReadLine().Trim();
                if (name.Length > 20) {
                    Utils.WriteLineColored($"Your name exceeds the limit by {name.Length - 20} chars!",
                        foreground: ConsoleColor.Red);
                    continue;
                }
            } while (false);

            lb.AddScore(new LeaderboardScore {
                PlayerName = name,
                Score = totalScore,
                Time = DateTime.UtcNow
            });

            lb.Display();
        }

        /// <summary>
        /// Blockingly read stdin for a single dimension (width/height).
        /// </summary>
        /// <returns>The dimension, -1 if no input.</returns>
        static int ReadDimension(string prompt) {
            int parsedDim = -1;
            while (true) {
                // Display prompt, usually asking for input
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(prompt);
                Console.ResetColor();

                string input = Console.ReadLine().Trim();
                if (input.Length == 0) {
                    // No input, let's go default
                    break;
                }

                int parsed;
                bool couldParse = int.TryParse(input, out parsed);
                if (!couldParse || parsed <= 0) {
                    Utils.WriteLineColored("Input must be an integer and larger than 0!",
                        foreground: ConsoleColor.Red);
                    continue;
                }

                // We set a defined limit to the maximum dimension
                if (parsed > MAX_DIMENSION) {
                    Utils.WriteLineColored($"Input must not be larger than {MAX_DIMENSION}!",
                        foreground: ConsoleColor.Red);
                    continue;
                }

                // Validated
                parsedDim = parsed;
                break;
            };

            return parsedDim;
        }

        static void Main(string[] args) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            // Create data folder
            System.IO.Directory.CreateDirectory(DataPaths.DataFolder);

            ProgramSettings pSettings = new ProgramSettings();
            GlobalLeaderboard actualGlb = new GlobalLeaderboard(DataPaths.LeaderboardFile);
            GlobalLeaderboard glb = actualGlb;

            // Helper anonymous function to create game
            Game makeGameFunc() => new(new GameOptions {
                DisableAdjacentRule = pSettings.IsDebugMode,
                AllowAllBuildingTypes = pSettings.IsDebugMode,
                GridWidth = pSettings.GridWidth,
                GridHeight = pSettings.GridHeight
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
                    DoLeaderboardEligibility(game, glb);
                })
                .AddOption("Load saved game", (m) => {
                    Game game = makeGameFunc();
                    game.Restore();
                    game.Play();
                    DoLeaderboardEligibility(game, glb);
                })
                .AddOption("Show high scores", (m) => {
                    Leaderboard lb = glb.GetLeaderboard(pSettings.GridWidth, pSettings.GridHeight);
                    lb.Display();
                })
                .AddHeading()
                .AddOption("Change city size", (m) => {
                    Console.WriteLine($"Your current city size is: {pSettings.GridWidth} \u00D7 {pSettings.GridHeight}");
                    Console.WriteLine("Please enter your new desired city size, or press enter to skip.");

                    int newWidth = ReadDimension($"New width, # columns: ({pSettings.GridWidth}) ");
                    int newHeight = ReadDimension($"New height, # rows: ({pSettings.GridHeight}) ");

                    // Set if requested
                    if (newWidth >= 0) pSettings.GridWidth = newWidth;
                    if (newHeight >= 0) pSettings.GridHeight = newHeight;
                })
                .AddOption("Toggle debug mode", (m) => {
                    pSettings.IsDebugMode = !pSettings.IsDebugMode;
                    Console.Write("Debug mode is now: ");
                    if (pSettings.IsDebugMode) {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write("on");
                        Console.ResetColor();

                        // Set glb to an arbitrary one with no file saving involved
                        glb = new GlobalLeaderboard(null);
                    } else {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write("off");
                        Console.ResetColor();

                        // Set glb back to the actual
                        glb = actualGlb;
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
