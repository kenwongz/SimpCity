using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using SimpCity.buildings;

namespace SimpCity {
    public class GameOptions {
        /// <summary>
        /// Disables the rule for adjacent placement of new buildings.
        /// </summary>
        public bool DisableAdjacentRule { get; set; } = false;
        /// <summary>
        /// Allows all available building types to be chosen
        /// If <i>false</i>, goes back to the default behavior of randomizing 2 building types.
        /// </summary>
        public bool AllowAllBuildingTypes { get; set; } = false;
        /// <summary>
        /// The configured width of the game's grid.
        /// (default <i>4</i>)
        /// </summary>
        public int GridWidth { get; set; } = 4;
        /// <summary>
        /// The configured height of the game's grid.
        /// (default <i>4</i>)
        /// </summary>
        public int GridHeight { get; set; } = 4;
    }

    public class Game {
        protected internal readonly GameOptions options;
        protected internal readonly IDictionary<BuildingTypes, BuildingInfo> buildingInfo;
        protected internal readonly CityGrid grid;
        protected internal int round;
        protected internal BuildingTypes[] currentBuildingPool;

        /// <summary>
        /// The max number of rounds that can be played in the game.
        /// </summary>
        protected internal int MaxRounds {
            get {
                return options.GridWidth * options.GridHeight;
            }
        }

        /// <summary>
        /// The configured width of the game's grid.
        /// </summary>
        public int GridWidth { get { return options.GridWidth; } }
        /// <summary>
        /// The configured height of the game's grid.
        /// </summary>
        public int GridHeight { get { return options.GridHeight; } }
        /// <summary>
        /// Whether the game has ended.
        /// </summary>
        public bool HasEnded {
            get {
                return round > MaxRounds;
            }
        }

        public Game(GameOptions options = null) {
            if (options == null) options = new GameOptions();
            this.options = options;

            grid = new CityGrid(options.GridWidth, options.GridHeight);
            round = 1;

            RefreshCurrentBuildingPool();

            // Exhaustive list of buildings and their operations
            buildingInfo = new Dictionary<BuildingTypes, BuildingInfo>() {
                {
                    BuildingTypes.Beach, new BuildingInfo() {
                        Code = Beach.Code,
                        Name = Beach.Name,
                        MakeNew = () => new Beach(buildingInfo[BuildingTypes.Beach])
                    }
                },
                {
                    BuildingTypes.Factory, new BuildingInfo() {
                        Code = Factory.Code,
                        Name = Factory.Name,
                        MakeNew = () => new Factory(buildingInfo[BuildingTypes.Factory])
                    }
                },
                {
                   BuildingTypes.House, new BuildingInfo() {
                       Code = House.Code,
                       Name = House.Name,
                       MakeNew = () => new House(buildingInfo[BuildingTypes.House])
                   }
                },
                {
                    BuildingTypes.Shop, new BuildingInfo() {
                        Code = Shop.Code,
                        Name = Shop.Name,
                        MakeNew = () => new Shop(buildingInfo[BuildingTypes.Shop])
                    }
                },
                {
                   BuildingTypes.Highway, new BuildingInfo() {
                       Code = Highway.Code,
                       Name = Highway.Name,
                       MakeNew = () => new Highway(buildingInfo[BuildingTypes.Highway])
                   }
                },
                {
                   BuildingTypes.Park, new BuildingInfo() {
                       Code = Park.Code,
                       Name = Park.Name,
                       MakeNew = () => new Park(buildingInfo[BuildingTypes.Park])
                   }
                },
                {
                   BuildingTypes.Monument, new BuildingInfo() {
                       Code = Monument.Code,
                       Name = Monument.Name,
                       MakeNew = () => new Monument(buildingInfo[BuildingTypes.Monument])
                   }
                }

            };

            // Automatically assign Type, CopiesLeft & Grid
            foreach (var item in buildingInfo) {
                item.Value.Type = item.Key;
                // C# does floor division implicitly on integers
                item.Value.CopiesLeft = GridWidth * GridHeight / 2;
                item.Value.Grid = grid;
            }
        }

        /// <summary>
        /// Converts a user input in "A1".."D4" coordinate form into a grid position,
        /// </summary>
        /// <exception cref="System.ArgumentException">When the input is not a supported coordinate format</exception>
        protected internal static CityGridPosition InputToPos(string inputPos) {
            if (inputPos == null) {
                throw new ArgumentException("Input not a supported coordinate format: " + inputPos ?? "(null)");
            }

            // Sanitise first
            inputPos = inputPos.Trim().ToUpper();
            if (inputPos.Length != 2) {
                throw new ArgumentException("Input not a supported coordinate format: " + inputPos ?? "(null)");
            }

            // Convert letter to positional index
            int x = inputPos[0] - 'A';
            int y = inputPos[1] - '1';

            return new CityGridPosition(x, y);
        }

        /// <summary>
        /// Chooses a random building type.
        /// </summary>
        protected internal static BuildingTypes RandomBuildingType() {
            Array values = Enum.GetValues(typeof(BuildingTypes));
            return (BuildingTypes)values.GetValue(ScRandom.GetInstance().Next(values.Length));
        }

        /// <summary>
        /// Refreshes the current building pool.
        /// </summary>
        protected internal void RefreshCurrentBuildingPool() {
            currentBuildingPool = new BuildingTypes[2] { RandomBuildingType(), RandomBuildingType() };
        }

        /// <summary>
        /// Display an ASCII wizardry..
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected void DisplayGrid() {
            for (int y = 0; y < grid.Height; y++) {

                // Print the horizontal heading
                if (y == 0) {
                    for (int x = 0; x < grid.Width; x++) {
                        char alphabet = (char)('A' + x);
                        Console.Write("     " + alphabet.ToString());
                    }
                    Console.WriteLine("");
                }

                Console.WriteLine("  " + Utils.RepeatString("+-----", grid.Width) + "+");

                for (int x = 0; x < grid.Width; x++) {

                    // Print the vertical heading
                    if (x == 0) {
                        Console.Write(y + 1);
                    }
                    var building = grid.Get(new CityGridPosition(x, y));
                    Console.Write(" | " + (building?.Info.Code ?? "   "));
                }

                // Enclosure and line break
                Console.WriteLine(" |");
            }

            // Enclosure
            Console.WriteLine("  " + Utils.RepeatString("+-----", grid.Width) + "+");
        }

        /// <summary>
        /// Calculates scores per building and building type.
        /// </summary>
        protected internal Dictionary<BuildingTypes, List<int>> CalculateScores() {
            // Create an empty map for the scores
            var buildingScores = new Dictionary<BuildingTypes, List<int>>();
            foreach (var entry in buildingInfo) {
                buildingScores[entry.Value.Type] = new List<int>();
            }

            // Calculate and archive
            ScoreCalculationArchive calcArchive = new ScoreCalculationArchive();
            for (int y = 0; y < grid.Height; y++) {
                for (int x = 0; x < grid.Width; x++) {
                    try {
                        CityGridBuilding b = grid.Get(new CityGridPosition(x, y));
                        if (b != null && !calcArchive.IsCalculated(b)) {
                            buildingScores[b.Info.Type].Add(b.CalcScore(calcArchive));

                            // Set as calculated, if not already done by CalcScore
                            if (!calcArchive.IsCalculated(b)) {
                                calcArchive.Calculated(b);
                            }
                        } else {
                            // Ignore null building or calculated buildings
                            continue; 
                        }
                    } catch (Exception ex) {
                        // TODO: Temporary catching while US-8 is in progress.
                        Utils.WriteLineColored(ex.Message,
                            foreground: ConsoleColor.Red);
                    }
                }
            }

            return buildingScores;
        }

        /// <summary>
        /// Displays the current score.
        /// </summary>
        protected void DisplayScores() {
            // Score display
            int totalScore = 0;
            foreach (var entry in CalculateScores()) {
                string formattedFormula = string.Join(" + ", entry.Value);
                int score = 0;
                foreach (int s in entry.Value) {
                    score += s;
                }
                totalScore += score;

                // Display per-type calculation
                Console.WriteLine($"{buildingInfo[entry.Key].Code}: {formattedFormula}{(entry.Value.Count <= 0 ? "" : " = ")}{score}");
            }

            // Display the total score
            Console.WriteLine($"Total score: {totalScore}");
        }

        /// <summary>
        /// Builds a new building at the specified position.
        /// Increments the round count by one.
        /// </summary>
        /// <returns>The building that was just placed.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// <list type="bullet">
        /// <item>When the building already has a spot in the grid</item>
        /// <item>When the building is NOT placed orthogonally adjacent to another building after the first round</item>
        /// </list>
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">When the position is out of bounds</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">When the position is already occupied</exception>
        protected internal CityGridBuilding BuildAt(BuildingInfo info, CityGridPosition pos) {
            CityGridBuilding building = info.MakeNew();

            // Propagate any errors forward. Prioritise these exceptions.
            grid.PassiveAdd(building, pos);

            if (round > 1 && !(options?.DisableAdjacentRule ?? false)) {
                // After the first round, check to ensure that there is at least one building in the adjacent position.
                bool hasAdjacent = false;
                foreach (CityGridOffset offset in CityGrid.AdjacentOffsets()) {
                    CityGridPosition offsetPos = pos.Offset(offset);
                    if (grid.IsWithin(offsetPos) && grid.Get(offsetPos) != null) {
                        hasAdjacent = true;
                        break;
                    }
                }
                if (!hasAdjacent) {
                    throw new InvalidOperationException("You must build next to an existing building.");
                }
            }

            // All errors have been caught prior, let's skip the check with force = true
            grid.Add(building, pos, true);
            round++;

            return building;
        }

        /// <summary>
        /// Query player for the position to build.
        /// This is triggered when the player chooses <i>Build &lt;X></i> option in the game.
        /// </summary>
        protected void OnMakeMove(BuildingInfo info) {
            // Gather coordinate from input
            CityGridPosition pos;
            while (true) {
                Console.Write("Choose the position to build: ");
                try {
                    pos = InputToPos(Console.ReadLine());
                    break;
                } catch (ArgumentException ex) {
                    Utils.WriteLineColored(ex.Message,
                        foreground: ConsoleColor.Red);
                    continue;
                }
            }

            // Build at the specified position
            try {
                BuildAt(info, pos);
            } catch (Exception ex) {
                Utils.WriteLineColored(ex.Message,
                    foreground: ConsoleColor.Red);
                return;
            }

            // Randomize the building pool again.
            RefreshCurrentBuildingPool();
        }

        /// <summary>
        /// Saves the grid data structure into the file.
        /// </summary>
        public void Save() {
            StringBuilder csvBuilder = new StringBuilder();
            for (int y = 0; y < grid.Height; y++) {
                List<string> rowBuildings = new List<string>();
                for (int x = 0; x < grid.Width; x++) {
                    var b = grid.Get(new CityGridPosition(x, y));
                    rowBuildings.Add(b?.Info.Code ?? "x");
                }
                csvBuilder.AppendLine(string.Join(",", rowBuildings));
            }

            // Open the file stream with append = false
            StreamWriter csvStream = new StreamWriter("Grid.csv", false);
            csvStream.WriteLine(csvBuilder.ToString());
            csvStream.Close();

            Console.WriteLine("Game saved successfully");
        }

        /// <summary>
        /// Loads the grid data from file into the grid data structure.
        /// </summary>
        public void Restore() {
            int count = 0;
            int colcount = 0;

            // Read the file lines into memory.
            // Stream is automatically closed at the end of the scope.
            List<string> glist = new List<string>();
            using (StreamReader csvReader = new StreamReader("Grid.csv")) {
                var line = csvReader.ReadLine();
                while (line != null) {
                    glist.Add(line);
                    line = csvReader.ReadLine();
                }
            }

            foreach (var line in glist) {
                count += 1;
                string rowlist = line.ToString();
                string[] row = rowlist.Split(',');
                foreach (var column in row) {
                    colcount += 1;
                    string col = column.ToString();
                    if (col == "BCH") {
                        var b = buildingInfo[BuildingTypes.Beach].MakeNew();
                        grid.Add(b, new CityGridPosition(colcount - 1, count - 1));

                    }
                    if (col == "FAC") {
                        var c = buildingInfo[BuildingTypes.Factory].MakeNew();
                        grid.Add(c, new CityGridPosition(colcount - 1, count - 1));

                    }
                    if (col == "SHP") {
                        var d = buildingInfo[BuildingTypes.Shop].MakeNew();
                        grid.Add(d, new CityGridPosition(colcount - 1, count - 1));

                    }
                    if (col == "HWY") {
                        var d = buildingInfo[BuildingTypes.Highway].MakeNew();
                        grid.Add(d, new CityGridPosition(colcount - 1, count - 1));

                    }
                    if (col == "MON") {
                        var d = buildingInfo[BuildingTypes.Monument].MakeNew();
                        grid.Add(d, new CityGridPosition(colcount - 1, count - 1));

                    }
                    if (col == "PRK") {
                        var d = buildingInfo[BuildingTypes.Park].MakeNew();
                        grid.Add(d, new CityGridPosition(colcount - 1, count - 1));

                    }
                    if (col == "HSE") {
                        var b = buildingInfo[BuildingTypes.House].MakeNew();
                        grid.Add(b, new CityGridPosition(colcount - 1, count - 1));

                    }

                }
                colcount = 0;

            }

        }

        /// <summary>
        /// Starts the game blockingly.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public void Play() {
            var menu = new ConsoleMenu()
                .BeforeInteraction((m) => {
                    if (round > MaxRounds) {
                        // Prepare to exit the game.
                        Console.WriteLine("Final layout of Simp City:");
                        DisplayGrid();
                        DisplayScores();
                        m.Exit();
                        return;
                    }

                    // Display the round number
                    Console.WriteLine($"Turn {round} ({MaxRounds - round + 1} left)");
                    // Display the current grid
                    DisplayGrid();

                    if (options?.AllowAllBuildingTypes ?? false) {
                        // Replace all placeholders with the all the building types
                        int loopCount = 0;
                        foreach (var item in buildingInfo) {
                            loopCount++;
                            m.EditOption(
                                loopCount.ToString(), string.Format("Build a {0}", item.Value.Code),
                                // Trigger the make-move event
                                (_) => OnMakeMove(item.Value)
                            );
                        }
                    } else {
                        // Show the current building pool
                        for (int i = 1; i < 3; i++) {
                            var chosen = currentBuildingPool[i - 1];
                            // Replace the menu option
                            m.EditOption(
                                i.ToString(), string.Format("Build a {0}", buildingInfo[chosen].Code),
                                // Trigger the make-move event
                                (_) => OnMakeMove(buildingInfo[chosen])
                            );
                        }
                    }
                });

            // Counts the number of times a placeholder should be made on the menu
            int placeholderCount = (options?.AllowAllBuildingTypes ?? false) ? buildingInfo.Count : 2;
            for (int i = 0; i < placeholderCount; i++) {
                // Stub placeholder, its description does not really matter
                menu.AddOption(null);
            }

            // Add in the remaining options
            menu.AddOption("See remaining buildings", (m) => Console.WriteLine("Todo remaining building"))
                .AddOption("See current score", (m) => {
                    DisplayScores();
                })
                .AddHeading()
                .AddOption("Save game", (m) => {
                    Save();
                })
                .AddExitOption("Exit to main menu");

            menu.DisplayInteraction();

        }
    }
}
