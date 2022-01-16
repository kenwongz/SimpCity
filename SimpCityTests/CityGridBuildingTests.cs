using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpCity;

namespace SimpCityTests {
    [TestClass]
    public class CityGridBuildingTests {
        /// <summary>
        /// N/A, US-8a:
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

        /// <summary>
        /// N/A, US-8a:
        /// This ensures correct score calculation for House.
        /// </summary>
        [TestMethod]
        public void CalcScore_CalculatesCorrectly_ForHouse() {
            // Requirement: If a House (HSE) is next to a factory (FAC), then it scores 1 point only. Otherwise, it
            // scores 1 point for each adjacent house(HSE) or shop(SHP), and 2 points for each
            // adjacent beach(BCH)

            CityGridBuilding b;
            ScoreCalculationArchive a = new ScoreCalculationArchive();  // stub

            Game game = new Game();
            // Build a factory
            game.BuildAt(game.buildingInfo[BuildingTypes.Factory], new CityGridPosition(0, 0));
            // Build a house next to it
            b = game.BuildAt(game.buildingInfo[BuildingTypes.House], new CityGridPosition(1, 0));
            // Build a distraction (beach) next to the house to make sure it isn't prioritised
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(2, 0));

            // Ensure 1 points
            Assert.IsTrue(b.CalcScore(a) == 1);

            // Part 2: start a new game
            game = new Game();
            // Build a house somewhere in the center
            b = game.BuildAt(game.buildingInfo[BuildingTypes.House], new CityGridPosition(1, 1));
            // Build a shop and house next to it
            game.BuildAt(game.buildingInfo[BuildingTypes.Shop], new CityGridPosition(0, 1));
            game.BuildAt(game.buildingInfo[BuildingTypes.House], new CityGridPosition(2, 1));

            // Ensure 2 points
            Assert.IsTrue(b.CalcScore(a) == 2);

            // Now build a beach next to the house
            game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(1, 0));
            // Ensure 4 cumulative points
            Assert.IsTrue(b.CalcScore(a) == 4);
        }
    }
}
