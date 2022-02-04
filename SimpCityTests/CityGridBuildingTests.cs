using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpCity;

namespace SimpCityTests {
    [TestClass]
    public class CityGridBuildingTests {
        /// <summary>
        /// This ensures correct score calculation for Beach.
        /// </summary>
        [TestMethod]
        public void CalcScore_CalculatesCorrectly_ForBeach() {
            // Test various widths, leaving at least one column in the middle for testing
            for (int width = 3; width <= 10; width++) {
                Game game = new Game(new GameOptions {
                    // Disable adjacent rule for easier testing
                    DisableAdjacentRule = true,
                    // Use a custom grid width length
                    GridWidth = width
                });

                // Requirement: A Beach (BCH) scores 3 points if it is built in column A or the
                // last column, or 1 point otherwise

                CityGridBuilding lastBuilding;
                ScoreCalculationArchive a = new ScoreCalculationArchive();  // stub

                // Build on col A (first col)
                lastBuilding = game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(0, 0));
                // Ensure 3 points
                Assert.IsTrue(lastBuilding.CalcScore(a) == 3);

                // Build on col B (middle)
                lastBuilding = game.BuildAt(game.buildingInfo[BuildingTypes.Beach], new CityGridPosition(1, 0));
                // Ensure 1 points
                Assert.IsTrue(lastBuilding.CalcScore(a) == 1);

                // Build on last col
                lastBuilding = game.BuildAt(game.buildingInfo[BuildingTypes.Beach],
                        new CityGridPosition(width - 1, 0));
                // Ensure 3 points
                Assert.IsTrue(lastBuilding.CalcScore(a) == 3);
            }
        }

        /// <summary>
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

        /// <summary>
        /// This ensures correct score calculation for Park.
        /// </summary>
        [TestMethod]
        public void CalcScore_CalculatesCorrectly_ForPark() {
            // Requirement: A Park is a new type of building depending on how many
            // parks are connected to each other(both horizontally and vertically). The
            // score for a Park is given by the following table
            // Size  1 2 3 4 5 6 7 8
            // Score 1 3 8 16 22 23 24 25

            // 3*3 < 8 (needed to test up till size 8
            Game game = new Game(new GameOptions {
                GridWidth = 3,
                GridHeight = 3
            });

            // Let's simulate this set up (25 points)
            // PRK PRK PRK
            // PRK PRK PRK
            // PRK PRK nul
            TestUtils.BatchGameBuild(game, new BuildingTypes?[3,3] {
                { BuildingTypes.Park, BuildingTypes.Park, BuildingTypes.Park },
                { BuildingTypes.Park, BuildingTypes.Park, BuildingTypes.Park },
                { BuildingTypes.Park, BuildingTypes.Park, null },
            });

            // Some sanity check
            Assert.IsTrue(game.grid.Get(new CityGridPosition(0, 0)).Info.Code == "PRK"
                && game.grid.Get(new CityGridPosition(2, 2)) == null);

            // Check that we got 25 points
            Assert.IsTrue(TestUtils.TotalOf(game.CalculateScores()[BuildingTypes.Park]) == 25);

            // Now let's add the last park, which no longer continues in the chain of 8 parks.
            // Therefore, the last park adds +1 score, which means 26 in total.
            game.BuildAt(game.buildingInfo[BuildingTypes.Park], new CityGridPosition(2, 2));
            Assert.IsTrue(TestUtils.TotalOf(game.CalculateScores()[BuildingTypes.Park]) == 26);
        }
    }
}
