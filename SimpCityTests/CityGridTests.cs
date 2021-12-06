using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpCity;
using SimpCity.buildings;
using System;
using System.Collections.Generic;

namespace SimpCityTests {

    [TestClass]
    public class CityGridTests {
        // Building info stub
        private IDictionary<BuildingTypes, BuildingInfo> buildingInfo;

        [TestInitialize]
        public void CreateBuildingInfo() {
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
                //item.Value.CopiesLeft = BUILDING_COPIES;
                //item.Value.Grid = grid;
            }
            
        }

        /// <summary>
        /// Grid test utility to easily assert building type in the grid.
        /// </summary>
        /// <param name="compare">A multidimensional array of 3-letter codes of the building to compare against.</param>
        /// <example>
        /// IsEqual(grid, {
        ///      { "HSE", null },
        ///      { null, "BCH" }
        /// });
        /// </example>
        public bool IsEqual(CityGrid grid, string[,] compare) {
            for (int y = 0; y < grid.Height; y++) {
                for (int x = 0; x < grid.Width; x++) {
                    CityGridBuilding b = grid.Get(new CityGridPosition(x, y));
                    if (string.IsNullOrEmpty(compare[x, y])) {
                        if (b is null) {
                            continue;
                        } else {
                            return false;
                        }
                    }
                    if (b.Info.Code != compare[x, y]) {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Tests if the test utility equality method is working.
        /// </summary>
        [TestMethod]
        public void TestIsEquals_ReturnsTrue_WhenEqual() {
            CityGrid cg = new CityGrid(4, 4);
            cg.Add(buildingInfo[BuildingTypes.Beach].MakeNew(), new CityGridPosition(0, 3));
            cg.Add(buildingInfo[BuildingTypes.Beach].MakeNew(), new CityGridPosition(3, 0));
            Assert.IsTrue(IsEqual(cg, new string[4,4] {
                { null, null, null, "BCH" },
                { null, null, null, null },
                { null, null, null, null },
                { "BCH", null, null, null },
            }));
        }

        /// <summary>
        /// Tests if the test utility equality method is working.
        /// </summary>
        [TestMethod]
        public void TestIsEquals_ReturnsFalse_WhenNotEqual() {
            CityGrid cg = new CityGrid(4, 4);
            cg.Add(buildingInfo[BuildingTypes.Beach].MakeNew(), new CityGridPosition(0, 3));
            cg.Add(buildingInfo[BuildingTypes.Beach].MakeNew(), new CityGridPosition(3, 0));
            Assert.IsFalse(IsEqual(cg, new string[4, 4] {
                { null, null, "BCH", null },
                { null, null, null, null },
                { null, null, null, null },
                { null, "BCH", null, null },
            }));
        }
    }
}
