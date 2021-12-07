using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpCity;
using System;

namespace SimpCityTests {
    /// <summary>
    /// Tests the gameplay
    /// </summary>
    [TestClass]
    public class GameTests {
        /// <summary>
        /// QA-SN-5.1:
        /// This ensures that the game can place buildings down.
        /// </summary>
        [TestMethod]
        public void BuildAt_PlacesBuilding_WhenCalledProperly() {
            Game game = new Game();

            // Build a beach at 0,1
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 1));

            // Ensure that the building is at 0,1
            Assert.IsTrue(TestUtils.IsGridEqual(game.grid, new string[4, 4] {
                { null, "BCH", null, null },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
            }));
        }

        /// <summary>
        /// N/A:
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
        /// N/A:
        /// This ensures that a position input is correctly mapped.
        /// </summary>
        [TestMethod]
        public void InputToPos_ReturnsCorrectly_WhenInputProperly() {
            // Check at 0,0
            CityGridPosition pos = Game.InputToPos("A1");
            Assert.IsTrue(pos.X == 0 && pos.Y == 0);

            // Check at 2,3
            pos = Game.InputToPos("C4");
            Assert.IsTrue(pos.X == 2 && pos.Y == 3);
        }
    }
}
