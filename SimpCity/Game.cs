using System;
using System.Collections.Generic;
using System.IO;
using SimpCity.buildings;

namespace SimpCity {
    public class Game {
        private const int GRID_WIDTH = 4;
        private const int GRID_HEIGHT = 4;
        private const int MAX_ROUNDS = GRID_WIDTH * GRID_HEIGHT;
        private const int BUILDING_COPIES = 8;

        protected internal readonly IDictionary<BuildingTypes, BuildingInfo> buildingInfo;
        protected internal readonly CityGrid grid;
        protected internal int round;

        public Game() {
            grid = new CityGrid(GRID_WIDTH, GRID_HEIGHT);
            round = 0;

            // Exhaustive list of buildings and their operations
            buildingInfo = new Dictionary<BuildingTypes, BuildingInfo>() {
                {
                    BuildingTypes.Beach, new BuildingInfo() {
                        Code = Beach.Code,
                        Name = Beach.Name,
                        MakeNew = () => new Beach(buildingInfo[BuildingTypes.Beach])
                    }
                }
            };

            // Automatically assign Type, CopiesLeft & Grid
            foreach (var item in buildingInfo) {
                item.Value.Type = item.Key;
                item.Value.CopiesLeft = BUILDING_COPIES;
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
        /// Display an ASCII wizardry..
        /// </summary>
        protected void DisplayGrid() {
            Console.WriteLine("Turn " + round);

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
        /// Builds a new building at the specified position.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">When the building already has a spot in the grid</exception>
        /// <exception cref="System.IndexOutOfRangeException">When the position is out of bounds</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">When the position is already occupied</exception>
        protected internal void BuildAt(BuildingInfo info, CityGridPosition pos) {
            info.MakeNew().Add(pos);
        }

        /// <summary>
        /// This is triggered when the player chooses "Build <X>" option in the game.
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    continue;
                }
            }

            // Build at the specified position
            try {
                BuildAt(info, pos);
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            
        }

        public void Save() {
            // TODO: US-3 - save state
            CityGridBuilding[,] rawGrid = grid.GetRawGrid();

            for (int y = 0; y < grid.Height; y++) {
                //line = "";
                for (int x = 0; x < grid.Width; x++) {
                    var b = grid.Get(new CityGridPosition(x, y));
                    //if (b is null) { "x"} else if (b is Beach) { "BCH"}
                    //line += "," + ;
                }
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// Loads the grid data from file into the grid data structure.
        /// </summary>
        public void Restore() {
            int count = 0;
            int colcount = 0;
            IEnumerable<string> glist = File.ReadLines("Grid.csv");
            foreach (var line in File.ReadLines("Grid.csv")) {
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

                }
                colcount = 0;
                //Do something

            }


            // TODO: US-5 - restore state
            // grid = filesaver.load()
            // test
        }

        public void Play() {
            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction((m) => {
                    // Display the current grid
                    DisplayGrid();
                    // Choose random 2 buildings
                    for (int i = 1; i < 3; i++) {
                        BuildingTypes chosen = RandomBuildingType();
                        // Replace the menu option
                        m.EditOption(
                            i.ToString(), string.Format("Build a {0}", buildingInfo[chosen].Code),
                            // Create a new building object and make the move
                            (_) => OnMakeMove(buildingInfo[chosen])
                        );
                    }
                })
                // These two placeholders will be edited accordingly before each interaction
                .AddOption("Placeholder 1")
                .AddOption("Placeholder 2")
                .AddOption("See remaining buildings", (m) => Console.WriteLine("Todo remaining building"))
                .AddOption("See current score", (m) => Console.WriteLine("Todo curr score"))
                .AddHeading()
                .AddOption("Save game", (m) => Console.WriteLine("Todo save game"))
                .AddExitOption("Exit to main menu");

            menu.DisplayInteraction();

        }
    }
}
