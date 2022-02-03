using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpCity;

namespace SimpCityTests {
    /// <summary>
    /// Tests the gameplay
    /// </summary>
    [TestClass]
    public class GameTests {
        /// <summary>
        /// This ensures that the game can place buildings down.
        /// </summary>
        [TestMethod]
        public void BuildAt_PlacesBuilding_WhenCalledProperly() {
            Game game = new Game();

            // Building various buildings
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 1));
            game.BuildAt(game.buildingInfo[BuildingTypes.Factory], new CityGridPosition(1, 1));
            game.BuildAt(game.buildingInfo[BuildingTypes.Highway], new CityGridPosition(0, 0));
            game.BuildAt(game.buildingInfo[BuildingTypes.House], new CityGridPosition(1, 0));
            game.BuildAt(game.buildingInfo[BuildingTypes.Monument], new CityGridPosition(0, 3));
            game.BuildAt(game.buildingInfo[BuildingTypes.Park], new CityGridPosition(1, 2));
            game.BuildAt(game.buildingInfo[BuildingTypes.Shop], new CityGridPosition(2, 0));

            // Ensures that the building is at specified coordinates above.
            Assert.IsTrue(TestUtils.IsGridEqual(game.grid, new string[4, 4] {
                { "HWY", "BCH", "MON", null },
                { "HSE", "FAC", "PRK", null },
                { "SHP", null , null, null },
                { null, null, null, null },
            }));
        }


        /// <summary>
        /// This ensures that the function will throw an error when an existing position is given.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Trying to place a building on an existing one did not flag out..")]
        public void BuildAt_Throws_WhenBuildingExists() {
            Game game = new Game();

            // Build a beach at 0,1
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 1));

            // Build a beach at 0,1 again ; this should throw
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 1));
        }

        /// <summary>
        /// This ensures that the function will throw an error after the first round, when attempting
        /// to place at a position with no adjacent buildings.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Trying to place a building not adjacent to another, did not flag out..")]
        public void BuildAt_Throws_WhenNotAdjacentAfterFirstRound() {
            Game game = new Game();

            // Build a beach at 0,1
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 1));

            // On the second round, build a beach at 2,1 which is not adjacent to the above;
            // this should throw.
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(2, 1));
        }

        /// <summary>
        /// This ensures that the game will increment the round counter properly.
        /// </summary>
        [TestMethod]
        public void BuildAt_IncrementsRoundCount_WhenCalledProperly() {
            Game game = new Game();

            // We must start with a round count of one
            Assert.IsTrue(game.round == 1);

            // Build a beach at 0,1
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 1));

            // We must now have with a round count of two
            Assert.IsTrue(game.round == 2);

            // On the second round, build a beach at 1,1
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(1, 1));

            // We must now have with a round count of three
            Assert.IsTrue(game.round == 3);
        }

        /// <summary>
        /// This ensures that a position input is correctly mapped.
        /// </summary>
        [TestMethod]
        public void InputToPos_ReturnsCorrectly_WhenInputProperly() {
            // Check at 0,0
            CityGridPosition pos = Game.InputToPos("A1");
            Assert.IsTrue(pos.X == 0 && pos.Y == 0);

            // Check at 2,3 ; with lowercase alphabet
            pos = Game.InputToPos("c4");
            Assert.IsTrue(pos.X == 2 && pos.Y == 3);
        }

        /// <summary>
        /// This ensures that the function will throw an error when an existing position is given.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Trying to supply an invalid coordinate format did not flag out..")]
        public void InputToPos_Throws_WhenInputWrongly() {
            Game.InputToPos("You are my sunshine.");
        }


        // Check if files get read properly
        // Default output will be BCH at 2,2
        [TestMethod]
        public void Fileread() {
            int count = 0;
            int colcount = 0;
            foreach (var line in File.ReadLines("Grid.csv")) {
                count += 1;
                string rowlist = line.ToString();
                string[] row = rowlist.Split(',');
                foreach (var column in row) {
                    colcount += 1;
                    string col = column.ToString();

                }
                colcount = 0;
                Console.WriteLine(line);
            }

        }
    }
}
