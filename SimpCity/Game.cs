using System;
using System.Collections.Generic;
using SimpCity.buildings;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpCity {
    public enum BuildingTypes {
        Beach
    }


    public delegate CityGridBuilding MakeNewFunc();
    public class BuildingInfo {
        public BuildingTypes Type { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public string Code { get; set; }
        public int CopiesLeft { get; set; }
        public CityGrid Grid { get; set; }
        /// <summary>
        /// The callback to execute to return the building object
        /// </summary>
        public MakeNewFunc MakeNew { get; set; }
    }

    public class Game {
        private const int GRID_WIDTH = 4;
        private const int GRID_HEIGHT = 4;
        private const int MAX_ROUNDS = GRID_WIDTH * GRID_HEIGHT;
        private const int BUILDING_COPIES = 8;

        private readonly IDictionary<BuildingTypes, BuildingInfo> buildingInfo;
        private readonly CityGrid grid;
        private int round;

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
            CityGridBuilding[,] rawGrid = grid.GetRawGrid();

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
                    var building = rawGrid[x, y];
                    Console.Write(" | " + (building?.Info.Code ?? "   "));
                }
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// This is triggered when the player chooses "Build <X>" option in the game.
        /// </summary>
        protected void MakeMove(BuildingInfo info) {
            throw new NotImplementedException();
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
