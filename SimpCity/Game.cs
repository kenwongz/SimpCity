using System;
using System.Collections.Generic;
using SimpCity.buildings;

namespace SimpCity {
    public enum BuildingTypes {
        Beach,
    }

    public class BuildingInfo {
        public BuildingTypes Type { get; set; }
        /// <summary>
        /// The 3-character code for the building
        /// </summary>
        public string Code { get; set; }
        public int CopiesLeft { get; set; }
        public Action MakeNew { get; set; }
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

            // Exhaustive list of buildings and their operations.
            // Type & CopiesLeft are automatically assigned after this.
            buildingInfo = new Dictionary<BuildingTypes, BuildingInfo>() {
                {
                    BuildingTypes.Beach, new BuildingInfo() {
                        Code = Beach.Code,
                        MakeNew = () => new Beach(grid)
                    }
                }
            };

            // Assign Type and CopiesLeft
            foreach (var item in buildingInfo) {
                item.Value.Type = item.Key;
                item.Value.CopiesLeft = BUILDING_COPIES;
            }
        }

        /// <summary>
        /// Saves the current grid data into the file.
        /// </summary>
        public void Save() {
            // TODO: US-3 - save state
            // filesaver.save(grid);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the grid data from file into the grid data structure.
        /// </summary>
        public void Restore() {
            // TODO: US-5 - restore state
            // grid = filesaver.load()
            throw new NotImplementedException();
        }

        /// <summary>
        /// Blockingly runs the game until the user explicitly exits.
        /// </summary>
        public void Play() {
            // TODO: US-2 - gameplay
            throw new NotImplementedException();
        }
    }
}
