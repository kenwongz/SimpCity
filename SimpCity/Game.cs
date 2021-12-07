using System;
using System.Collections.Generic;
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
                Console.WriteLine("");
            }
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
            throw new NotImplementedException();
        }

        public void Save() {
            // TODO: US-3 - save state
            throw new NotImplementedException();
        }

        public void Restore() {
            // TODO: US-5 - restore state
            throw new NotImplementedException();
        }

        public void Play() {
            ConsoleMenu menu = new ConsoleMenu()
                .BeforeInteraction((m) => {
                    // Display the current grid
                    DisplayGrid();
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
