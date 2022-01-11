using System.Collections.Generic;
using SimpCity;

namespace SimpCityTests {
    public class TestUtils {
        /// <summary>
        /// Grid test utility to easily assert building type in the grid.
        /// </summary>
        /// <param name="compare">A multidimensional array of 3-letter codes of the building to compare against.</param>
        /// <example>
        /// IsGridEqual(grid, {
        ///      { "HSE", null },
        ///      { null, "BCH" }
        /// });
        /// </example>
        public static bool IsGridEqual(CityGrid grid, string[,] compare) {
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
        /// Utility to get the sum of all integers in the list.
        /// </summary>
        public static int TotalOf(List<int> intList) {
            int total = 0;
            foreach (int n in intList) {
                total += n;
            }
            return total;
        }
    }
}
