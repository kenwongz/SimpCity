using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpCity;

namespace SimpCityTests {
    [TestClass]
    public class CityGridBuildingTests {
        /// <summary>
        /// N/A, US-8:
        /// This ensures correct score calculation for Beach.
        /// </summary>
        [TestMethod]
        public void CalcScore_CalculatesCorrectly_ForBeach() {
            Game game = new Game(new GameOptions {
                // Disable adjacent rule for easier testing
                DisableAdjacentRule = true
            });

            // Requirement: A Beach (BCH) scores 3 points if it is built in column A or column D,
            // or 1 point otherwise

            CityGridBuilding lastBuilding;
            ScoreCalculationArchive a = new ScoreCalculationArchive();  // stub

            // Build on col A
            lastBuilding = game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 0));
            // Ensure 3 points
            Assert.IsTrue(lastBuilding.CalcScore(a) == 3);

            // Build on col C
            lastBuilding = game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(2, 0));
            // Ensure 1 points
            Assert.IsTrue(lastBuilding.CalcScore(a) == 1);

            // Build on col D
            lastBuilding = game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(3, 0));
            // Ensure 3 points
            Assert.IsTrue(lastBuilding.CalcScore(a) == 3);
        }
    }
}
